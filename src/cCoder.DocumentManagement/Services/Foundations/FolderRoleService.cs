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

    public ValueTask<FolderRole> AddFolderRoleAsync(FolderRole folderRole)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [folderRole]);
            cCoder.Data.Models.Security.FolderRole newFolderRole = CreateStorageFolderRole(folderRole: folderRole);


            authorizationBroker.Authorize(
                appId: folderRoleBroker.GetAppId(entity: newFolderRole),
                privilege: $"{nameof(FolderRole)}_create"
            );


            FolderRole result = await folderRoleBroker.InsertFolderRoleAsync(entity: newFolderRole);

            folderRole.FolderId = result.FolderId;

            folderRole.RoleId = result.RoleId;

            return folderRole;

        });

    public ValueTask DeleteFolderRoleAsync(FolderRole folderRole)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [folderRole]);
            authorizationBroker.Authorize(
    appId: folderRoleBroker.GetAppId(entity: CreateStorageFolderRole(folderRole: folderRole)),
    privilege: $"{nameof(FolderRole)}_delete"
);


            _ = await folderRoleBroker.DeleteFolderRoleAsync(entity: CreateStorageFolderRole(folderRole: folderRole));

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