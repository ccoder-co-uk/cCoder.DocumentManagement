// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Brokers;
using cCoder.DocumentManagement.Brokers.Storage;
using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using IAuthorizationBroker = cCoder.DocumentManagement.Brokers.IAuthorizationBroker;


namespace cCoder.DocumentManagement.Services.Foundations;

internal partial class FolderRoleService(
    IFolderRoleBroker folderRoleBroker,
    IAuthorizationBroker authorizationBroker
) : IFolderRoleService
{
    private void Authorize(int? appId, string privilege)
    {
        User user = authorizationBroker.GetCurrentUser();
        string normalizedPrivilege = privilege.ToLowerInvariant();

        bool hasPrivilege = user?.Roles?.Any(predicate: userRole =>
            (appId is null || userRole.Role.AppId == appId)
            && userRole.Role.Privileges.Contains(item: normalizedPrivilege))
            ?? false;

        if (user is null
            || !(user.IsAdminOfApp(appId: appId) || hasPrivilege))
        {
            throw new System.Security.SecurityException(message: "Access Denied!");
        }
    }

    public IQueryable<FolderRole> GetAll(bool ignoreFilters = false)
=>
        TryCatch(operation: () =>
        {
            ValidateAllOnGet(ignoreFilters: ignoreFilters);
            return folderRoleBroker.SelectAllFolderRoles(ignoreFilters: ignoreFilters);
        });

    public ValueTask<FolderRole> AddFolderRoleAsync(FolderRole newFolderRole)
=>
        TryCatch(operation: async () =>
        {
            ValidateFolderRoleOnAdd(newFolderRole: newFolderRole);

            cCoder.Data.Models.Security.FolderRole storageFolderRole =
                CreateStorageFolderRole(folderRole: newFolderRole);


            Authorize(
                appId: folderRoleBroker.SelectAppId(folderRole: storageFolderRole),
                privilege: $"{nameof(FolderRole)}_create"
            );


            FolderRole result = await folderRoleBroker.InsertFolderRoleAsync(newFolderRole: storageFolderRole);

            newFolderRole.FolderId = result.FolderId;

            newFolderRole.RoleId = result.RoleId;

            return newFolderRole;

        });

    public ValueTask DeleteFolderRoleAsync(FolderRole deletedFolderRole)
=>
        TryCatch(operation: async () =>
        {
            ValidateFolderRoleOnDelete(deletedFolderRole: deletedFolderRole);

            Authorize(
    appId: folderRoleBroker.SelectAppId(folderRole: CreateStorageFolderRole(folderRole: deletedFolderRole)),
    privilege: $"{nameof(FolderRole)}_delete"
);


            _ = await folderRoleBroker.DeleteFolderRoleAsync(deletedFolderRole: CreateStorageFolderRole(folderRole: deletedFolderRole));

        });

    public bool CanCreateFolderRole(FolderRole folderRole) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [folderRole]);

            Role role = folderRoleBroker.SelectRole(
                roleId: folderRole.RoleId,
                ignoreFilters: true);


            Folder folder = folderRoleBroker.SelectFolder(
                folderId: folderRole.FolderId,
                ignoreFilters: true);

            return role is not null
                && folder is not null
                && UserCan(
                    folder: folder,
                    user: authorizationBroker.GetCurrentUser(),
                    privilege: "folderrole_create");
        });

    public bool CanDeleteFolderRole(FolderRole folderRole) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [folderRole]);

            Folder folder = folderRoleBroker.SelectFolder(
                folderId: folderRole.FolderId,
                ignoreFilters: true);


            return folder is not null
                && UserCan(
                    folder: folder,
                    user: authorizationBroker.GetCurrentUser(),
                    privilege: "folderrole_delete");
        });

    public bool FolderRoleExists(FolderRole folderRole) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [folderRole]);

            return folderRoleBroker.SelectAllFolderRoles(ignoreFilters: true)
                .Any(predicate: existing =>
                    existing.FolderId == folderRole.FolderId
                    && existing.RoleId == folderRole.RoleId);
        });

    private static bool UserCan(Folder folder, User user, string privilege)
    {
        Guid[] userRoles = user?.Roles?
            .Select(selector: role => role.RoleId)
            .ToArray() ?? [];

        return user.IsAdminOfApp(appId: folder.AppId)
            || (folder.Roles?
                .Where(predicate: folderRole =>
                    userRoles.Contains(value: folderRole.RoleId))
                .SelectMany(selector: folderRole =>
                    folderRole.Role?.Privileges ?? [])
                .Contains(value: privilege) ?? false);
    }

    private static cCoder.Data.Models.Security.FolderRole CreateStorageFolderRole(FolderRole folderRole)
    {
        if (folderRole == null)
        {
            return null;
        }

        return new cCoder.Data.Models.Security.FolderRole
        {
            FolderId = folderRole.FolderId,
            RoleId = folderRole.RoleId
        };
    }
}