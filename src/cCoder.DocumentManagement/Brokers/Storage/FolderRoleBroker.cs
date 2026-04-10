using cCoder.Data;
using cCoder.Data.Models.Security;
using Microsoft.EntityFrameworkCore;


namespace cCoder.DocumentManagement.Brokers.Storage;

public interface IFolderRoleBroker
{
    IQueryable<FolderRole> GetAllFolderRoles(bool ignoreFilters);
    ValueTask<FolderRole> AddFolderRoleAsync(FolderRole entity);
    ValueTask<int> DeleteFolderRoleAsync(FolderRole entity);
    ValueTask DeleteAllFolderRolesAsync(IEnumerable<FolderRole> items);
    int? GetAppId(FolderRole entity);
}

public class FolderRoleBroker(ICoreContextFactory coreContextFactory) : IFolderRoleBroker
{

    public IQueryable<FolderRole> GetAllFolderRoles(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        return ignoreFilters
            ? coreDataContext.FolderRoles.IgnoreQueryFilters()
            : coreDataContext.FolderRoles;
    }

    public async ValueTask<FolderRole> AddFolderRoleAsync(FolderRole entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        FolderRole result = (await coreDataContext.FolderRoles.AddAsync(entity)).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteFolderRoleAsync(FolderRole entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.FolderRoles.Remove(entity);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllFolderRolesAsync(IEnumerable<FolderRole> items)
    {
        if (items == null || !items.Any())
            return;

        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.FolderRoles.RemoveRange(items);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public int? GetAppId(FolderRole entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        return coreDataContext.Folders

            .Where(folder => folder.Id == entity.FolderId)
            .Select(folder => (int?)folder.AppId)
            .FirstOrDefault();

    }
}







