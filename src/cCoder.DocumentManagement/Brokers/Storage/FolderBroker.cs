using cCoder.Data;
using cCoder.Data.Models.DMS;
using Microsoft.EntityFrameworkCore;


namespace cCoder.DocumentManagement.Brokers.Storage;

public interface IFolderBroker
{
    IQueryable<Folder> GetAllFolders(bool ignoreFilters);
    Folder GetFolderWithRoles(Guid id, bool ignoreFilters);
    Folder GetFolderForUpdate(Guid id, bool ignoreFilters);
    Folder GetFolderByPath(int appId, string path, bool ignoreFilters);
    Folder GetFolderByPathWithRoles(int appId, string path, bool ignoreFilters);
    Folder GetFolderByPathWithParentAndRoles(int appId, string path, bool ignoreFilters);
    Folder GetFolderByPathWithRolesAndFilesAndContents(int appId, string path, bool ignoreFilters);
    Folder GetFolderByPathWithSubFoldersAndFiles(int appId, string path, bool ignoreFilters);
    ValueTask<Folder> AddFolderAsync(Folder entity);
    ValueTask<Folder> UpdateFolderAsync(Folder entity);
    ValueTask<int> DeleteFolderAsync(Folder entity);
    ValueTask DeleteAllFoldersAsync(IEnumerable<Folder> items);
    int? GetAppId(Folder entity);
}

public class FolderBroker(ICoreContextFactory coreContextFactory) : IFolderBroker
{

    public IQueryable<Folder> GetAllFolders(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        IQueryable<Folder> query = ignoreFilters
            ? coreDataContext.Folders.IgnoreQueryFilters()
            : coreDataContext.Folders;

        return query
            .Include(folder => folder.Files)
            .Include(folder => folder.SubFolders)
            .AsSplitQuery();
    }

    public Folder GetFolderWithRoles(Guid id, bool ignoreFilters)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        IQueryable<Folder> query = ignoreFilters
            ? coreDataContext.Folders.IgnoreQueryFilters()
            : coreDataContext.Folders;

        return query
            .Include(folder => folder.Roles)
                .ThenInclude(folderRole => folderRole.Role)
            .FirstOrDefault(folder => folder.Id == id);
    }

    public Folder GetFolderForUpdate(Guid id, bool ignoreFilters)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        IQueryable<Folder> query = ignoreFilters
            ? coreDataContext.Folders.IgnoreQueryFilters()
            : coreDataContext.Folders;

        return query
            .Include(folder => folder.App)
            .Include(folder => folder.SubFolders)
            .Include(folder => folder.Parent)
            .Include(folder => folder.Files)
            .Include(folder => folder.Roles)
                .ThenInclude(folderRole => folderRole.Role)
            .AsSplitQuery()
            .FirstOrDefault(folder => folder.Id == id);
    }

    public Folder GetFolderByPath(int appId, string path, bool ignoreFilters)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        IQueryable<Folder> query = ignoreFilters
            ? coreDataContext.Folders.IgnoreQueryFilters()
            : coreDataContext.Folders;

        return query.FirstOrDefault(folder => folder.AppId == appId && folder.Path == path);
    }

    public Folder GetFolderByPathWithRoles(int appId, string path, bool ignoreFilters)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        IQueryable<Folder> query = ignoreFilters
            ? coreDataContext.Folders.IgnoreQueryFilters()
            : coreDataContext.Folders;

        return query
            .Include(folder => folder.Roles)
                .ThenInclude(folderRole => folderRole.Role)
            .FirstOrDefault(folder => folder.AppId == appId && folder.Path == path);
    }

    public Folder GetFolderByPathWithParentAndRoles(int appId, string path, bool ignoreFilters)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        IQueryable<Folder> query = ignoreFilters
            ? coreDataContext.Folders.IgnoreQueryFilters()
            : coreDataContext.Folders;

        return query
            .Include(folder => folder.Parent)
            .Include(folder => folder.Roles)
                .ThenInclude(folderRole => folderRole.Role)
            .FirstOrDefault(folder => folder.AppId == appId && folder.Path == path);
    }

    public Folder GetFolderByPathWithRolesAndFilesAndContents(int appId, string path, bool ignoreFilters)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        IQueryable<Folder> query = ignoreFilters
            ? coreDataContext.Folders.IgnoreQueryFilters()
            : coreDataContext.Folders;

        return query
            .Include(folder => folder.Roles)
                .ThenInclude(folderRole => folderRole.Role)
            .Include(folder => folder.Files)
                .ThenInclude(file => file.Contents)
            .FirstOrDefault(folder => folder.AppId == appId && folder.Path == path);
    }

    public Folder GetFolderByPathWithSubFoldersAndFiles(int appId, string path, bool ignoreFilters)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        IQueryable<Folder> query = ignoreFilters
            ? coreDataContext.Folders.IgnoreQueryFilters()
            : coreDataContext.Folders;

        return query
            .Include(folder => folder.SubFolders)
            .Include(folder => folder.Files)
            .FirstOrDefault(folder => folder.AppId == appId && folder.Path == path);
    }

    public async ValueTask<Folder> AddFolderAsync(Folder entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Folder result = (await coreDataContext.Folders.AddAsync(entity)).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<Folder> UpdateFolderAsync(Folder entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Folder result = coreDataContext.Folders.Update(entity).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteFolderAsync(Folder entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Folder folder = await coreDataContext.Folders
            .IgnoreQueryFilters()
            .Include(foundFolder => foundFolder.Roles)
            .FirstOrDefaultAsync(foundFolder => foundFolder.Id == entity.Id);

        if (folder is null)
        {
            return 0;
        }

        if (folder.Roles?.Any() == true)
        {
            coreDataContext.FolderRoles.RemoveRange(folder.Roles);
        }

        coreDataContext.Folders.Remove(folder);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllFoldersAsync(IEnumerable<Folder> items)
    {
        if (items == null || !items.Any())
            return;

        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Guid[] folderIds = [.. items.Select(folder => folder.Id)];

        Folder[] folders = await coreDataContext.Folders
            .IgnoreQueryFilters()
            .Include(folder => folder.Roles)
            .Where(folder => folderIds.Contains(folder.Id))
            .ToArrayAsync();

        coreDataContext.FolderRoles.RemoveRange(
            folders.SelectMany(folder => folder.Roles ?? []));
        coreDataContext.Folders.RemoveRange(folders);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public int? GetAppId(Folder entity)
    {
        return entity.AppId;
    }
}







