// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
    public IQueryable<FolderRole> GetAll(bool ignoreFilters = false)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [ignoreFilters]);
            return folderRoleBroker.SelectAllFolderRoles(ignoreFilters: ignoreFilters);
        });

    public ValueTask<FolderRole> AddFolderRoleAsync(FolderRole newFolderRole)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [newFolderRole]);
            cCoder.Data.Models.Security.FolderRole storageFolderRole =
                CreateStorageFolderRole(folderRole: newFolderRole);


            authorizationBroker.Authorize(
                appId: folderRoleBroker.GetAppId(entity: storageFolderRole),
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
            ValidateInputs(inputs: [deletedFolderRole]);
            authorizationBroker.Authorize(
    appId: folderRoleBroker.GetAppId(entity: CreateStorageFolderRole(folderRole: deletedFolderRole)),
    privilege: $"{nameof(FolderRole)}_delete"
);


            _ = await folderRoleBroker.DeleteFolderRoleAsync(deletedFolderRole: CreateStorageFolderRole(folderRole: deletedFolderRole));

        });

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