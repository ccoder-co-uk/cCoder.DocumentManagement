using cCoder.DocumentManagement.Brokers.Storage;
using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using IAuthorizationBroker = cCoder.DocumentManagement.Brokers.IAuthorizationBroker;


namespace cCoder.DocumentManagement.Services.Foundations;

internal class FolderRoleService(
    IFolderRoleBroker folderRoleBroker,
    IAuthorizationBroker authorizationBroker
) : IFolderRoleService
{
    public IQueryable<FolderRole> GetAll(bool ignoreFilters = false) =>
        folderRoleBroker.GetAllFolderRoles(ignoreFilters);

    public async ValueTask<FolderRole> AddAsync(FolderRole folderRole)
    {
        cCoder.Data.Models.Security.FolderRole newFolderRole = CreateStorageFolderRole(folderRole);
        authorizationBroker.Authorize(
            folderRoleBroker.GetAppId(newFolderRole),
            $"{nameof(FolderRole)}_create"
        );

        FolderRole result = await folderRoleBroker.AddFolderRoleAsync(newFolderRole);
        folderRole.FolderId = result.FolderId;
        folderRole.RoleId = result.RoleId;
        return folderRole;
    }

    public async ValueTask DeleteAsync(FolderRole folderRole)
    {
        authorizationBroker.Authorize(
            folderRoleBroker.GetAppId(CreateStorageFolderRole(folderRole)),
            $"{nameof(FolderRole)}_delete"
        );
        _ = await folderRoleBroker.DeleteFolderRoleAsync(CreateStorageFolderRole(folderRole));
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












