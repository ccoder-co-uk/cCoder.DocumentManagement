using System.Security;
using cCoder.DocumentManagement.Brokers;
using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Foundations;
using Microsoft.EntityFrameworkCore;

namespace cCoder.DocumentManagement.Services.Processings;

internal class FolderRoleProcessingService(IFolderRoleService service, IRoleBroker roleBroker, IFolderService folderService, IAuthorizationBroker authorizationBroker) : IFolderRoleProcessingService
{
    private cCoder.Data.Models.Security.User User => authorizationBroker.GetCurrentUser();

    public IQueryable<cCoder.Data.Models.Security.FolderRole> GetAll(bool ignoreFilters = false)
    {
        return service.GetAll(ignoreFilters);
    }

    public ValueTask<cCoder.Data.Models.Security.FolderRole> AddAsync(cCoder.Data.Models.Security.FolderRole entity)
    {
        (cCoder.Data.Models.Security.Role, Folder) folderAndRole = GetFolderAndRole(entity);
        cCoder.Data.Models.Security.Role role = folderAndRole.Item1;
        Folder folder = folderAndRole.Item2;
        bool flag = role != null && folder != null;
        Func<Folder, cCoder.Data.Models.Security.Role, bool> func = (Folder currentFolder, cCoder.Data.Models.Security.Role currentRole) => currentFolder.UserCan(User, "folderrole_create");
        if (flag && func(folder, role))
        {
            ICollection<cCoder.Data.Models.Security.FolderRole> roles = folder.Roles;
            return (roles == null || !roles.Any((cCoder.Data.Models.Security.FolderRole r) => r.RoleId == role.Id)) ? service.AddAsync(entity) : ValueTask.FromResult(entity);
        }
        if (role != null && folder != null)
        {
            ICollection<cCoder.Data.Models.Security.FolderRole> folders = role.Folders;
            if (folders != null && folders.Any((cCoder.Data.Models.Security.FolderRole r) => r.FolderId == folder.Id))
            {
                return ValueTask.FromResult(entity);
            }
        }
        throw new SecurityException("Access Denied!");
    }

    public async ValueTask DeleteAsync(cCoder.Data.Models.Security.FolderRole link)
    {
        Folder folder = folderService.GetAll(ignoreFilters: true).FirstOrDefault((Folder u) => u.Id == link.FolderId);
        cCoder.Data.Models.Security.FolderRole dbVersion = service.GetAll(ignoreFilters: true).FirstOrDefault((cCoder.Data.Models.Security.FolderRole ur) => ur.RoleId == link.RoleId && ur.FolderId == link.FolderId);
        if (dbVersion == null || folder == null || !folder.UserCan(User, "folderrole_delete"))
        {
            throw new SecurityException("Access Denied!");
        }
        await service.DeleteAsync(dbVersion);
    }

    public async ValueTask<IEnumerable<Result<cCoder.Data.Models.Security.FolderRole>>> AddOrUpdate(IEnumerable<cCoder.Data.Models.Security.FolderRole> items)
    {
        cCoder.Data.Models.Security.FolderRole[] itemArray = items.ToArray();
        Guid[] leftIds = itemArray.Select((cCoder.Data.Models.Security.FolderRole item) => item.FolderId).Distinct().ToArray();
        cCoder.Data.Models.Security.FolderRole[] existingItems = (from item in GetAll()
                                                                       where ((ReadOnlySpan<Guid>)leftIds).Contains(item.FolderId)
                                                                       select item).ToArray();
        List<Result<cCoder.Data.Models.Security.FolderRole>> results = new List<Result<cCoder.Data.Models.Security.FolderRole>>();
        foreach (IGrouping<Guid, cCoder.Data.Models.Security.FolderRole> group in from item in itemArray
                                                                                       group item by item.FolderId)
        {
            cCoder.Data.Models.Security.FolderRole[] groupItems = group.ToArray();
            cCoder.Data.Models.Security.FolderRole[] existingGroupItems = existingItems.Where((cCoder.Data.Models.Security.FolderRole item) => object.Equals(item.FolderId, group.Key)).ToArray();
            await DeleteAllAsync(existingGroupItems);

            foreach (cCoder.Data.Models.Security.FolderRole item in groupItems)
            {
                try
                {
                    results.Add(new Result<cCoder.Data.Models.Security.FolderRole>
                    {
                        Id = $"{item.FolderId}:{item.RoleId}",
                        Success = true,
                        Item = await AddAsync(item),
                        Message = "Added Successfully"
                    });
                }
                catch (Exception ex)
                {
                    results.Add(new Result<cCoder.Data.Models.Security.FolderRole>
                    {
                        Id = $"{item.FolderId}:{item.RoleId}",
                        Success = false,
                        Item = item,
                        Message = ex.Message
                    });
                }
            }
        }
        return results;
    }

    public async ValueTask DeleteAllAsync(IEnumerable<cCoder.Data.Models.Security.FolderRole> items)
    {
        foreach (cCoder.Data.Models.Security.FolderRole item in items)
        {
            await DeleteAsync(item);
        }
    }

    private (cCoder.Data.Models.Security.Role role, Folder folder) GetFolderAndRole(cCoder.Data.Models.Security.FolderRole entity)
    {
        return (role: roleBroker.GetAllRoles(ignoreFilters: true).IgnoreQueryFilters().FirstOrDefault((cCoder.Data.Models.Security.Role r) => r.Id == entity.RoleId), folder: folderService.GetAll(ignoreFilters: true).FirstOrDefault((Folder u) => u.Id == entity.FolderId));
    }
}
