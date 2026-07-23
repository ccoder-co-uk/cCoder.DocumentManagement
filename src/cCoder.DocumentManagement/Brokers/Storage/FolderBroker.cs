// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
    ValueTask DeleteAllFoldersByAppIdAsync(int appId);
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
            .Include(navigationPropertyPath: folder => folder.Files)
            .Include(navigationPropertyPath: folder => folder.SubFolders)
            .AsSplitQuery();
    }

    public Folder GetFolderWithRoles(Guid id, bool ignoreFilters)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        IQueryable<Folder> query = ignoreFilters
            ? coreDataContext.Folders.IgnoreQueryFilters()
            : coreDataContext.Folders;

        return query
            .Include(navigationPropertyPath: folder => folder.Roles)
                .ThenInclude(navigationPropertyPath: folderRole => folderRole.Role)
            .FirstOrDefault(predicate: folder => folder.Id == id);
    }

    public Folder GetFolderForUpdate(Guid id, bool ignoreFilters)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        IQueryable<Folder> query = ignoreFilters
            ? coreDataContext.Folders.IgnoreQueryFilters()
            : coreDataContext.Folders;

        return query
            .Include(navigationPropertyPath: folder => folder.App)
            .Include(navigationPropertyPath: folder => folder.SubFolders)
            .Include(navigationPropertyPath: folder => folder.Parent)
            .Include(navigationPropertyPath: folder => folder.Files)
            .Include(navigationPropertyPath: folder => folder.Roles)
                .ThenInclude(navigationPropertyPath: folderRole => folderRole.Role)
            .AsSplitQuery()
            .FirstOrDefault(predicate: folder => folder.Id == id);
    }

    public Folder GetFolderByPath(int appId, string path, bool ignoreFilters)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        IQueryable<Folder> query = ignoreFilters
            ? coreDataContext.Folders.IgnoreQueryFilters()
            : coreDataContext.Folders;

        return query.FirstOrDefault(predicate: folder => folder.AppId == appId && folder.Path == path);
    }

    public Folder GetFolderByPathWithRoles(int appId, string path, bool ignoreFilters)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        IQueryable<Folder> query = ignoreFilters
            ? coreDataContext.Folders.IgnoreQueryFilters()
            : coreDataContext.Folders;

        return query
            .Include(navigationPropertyPath: folder => folder.Roles)
                .ThenInclude(navigationPropertyPath: folderRole => folderRole.Role)
            .FirstOrDefault(predicate: folder => folder.AppId == appId && folder.Path == path);
    }

    public Folder GetFolderByPathWithParentAndRoles(int appId, string path, bool ignoreFilters)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        IQueryable<Folder> query = ignoreFilters
            ? coreDataContext.Folders.IgnoreQueryFilters()
            : coreDataContext.Folders;

        return query
            .Include(navigationPropertyPath: folder => folder.Parent)
            .Include(navigationPropertyPath: folder => folder.Roles)
                .ThenInclude(navigationPropertyPath: folderRole => folderRole.Role)
            .FirstOrDefault(predicate: folder => folder.AppId == appId && folder.Path == path);
    }

    public Folder GetFolderByPathWithRolesAndFilesAndContents(int appId, string path, bool ignoreFilters)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        IQueryable<Folder> query = ignoreFilters
            ? coreDataContext.Folders.IgnoreQueryFilters()
            : coreDataContext.Folders;

        return query
            .Include(navigationPropertyPath: folder => folder.Roles)
                .ThenInclude(navigationPropertyPath: folderRole => folderRole.Role)
            .Include(navigationPropertyPath: folder => folder.Files)
                .ThenInclude(navigationPropertyPath: file => file.Contents)
            .FirstOrDefault(predicate: folder => folder.AppId == appId && folder.Path == path);
    }

    public Folder GetFolderByPathWithSubFoldersAndFiles(int appId, string path, bool ignoreFilters)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        IQueryable<Folder> query = ignoreFilters
            ? coreDataContext.Folders.IgnoreQueryFilters()
            : coreDataContext.Folders;

        return query
            .Include(navigationPropertyPath: folder => folder.SubFolders)
            .Include(navigationPropertyPath: folder => folder.Files)
            .FirstOrDefault(predicate: folder => folder.AppId == appId && folder.Path == path);
    }

    public async ValueTask<Folder> AddFolderAsync(Folder entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Folder result = (await coreDataContext.Folders.AddAsync(entity: entity)).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<Folder> UpdateFolderAsync(Folder entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Folder result = coreDataContext.Folders.Update(entity: entity).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteFolderAsync(Folder entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Folder folder = await coreDataContext.Folders
            .IgnoreQueryFilters()
            .Include(navigationPropertyPath: foundFolder => foundFolder.Roles)
            .FirstOrDefaultAsync(predicate: foundFolder => foundFolder.Id == entity.Id);

        if (folder is null)
        {
            return 0;
        }

        if (folder.Roles?.Any() == true)
        {
            coreDataContext.FolderRoles.RemoveRange(entities: folder.Roles);
        }

        coreDataContext.Folders.Remove(entity: folder);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllFoldersAsync(IEnumerable<Folder> items)
    {
        if (items == null || !items.Any())
        {
            return;
        }

        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Guid[] folderIds = [.. items.Select(selector: folder => folder.Id)];

        Folder[] folders = await coreDataContext.Folders
            .IgnoreQueryFilters()
            .Include(navigationPropertyPath: folder => folder.Roles)
            .Where(predicate: folder => folderIds.Contains(folder.Id))
            .ToArrayAsync();

        coreDataContext.FolderRoles.RemoveRange(
            entities: folders.SelectMany(folder => folder.Roles ?? []));
        coreDataContext.Folders.RemoveRange(entities: folders);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllFoldersByAppIdAsync(int appId)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        IQueryable<Guid> folderIds =
            coreDataContext.Folders
                .IgnoreQueryFilters()
                .Where(predicate: folder => folder.AppId == appId)
                .Select(selector: folder => folder.Id);

        IQueryable<Guid> fileIds =
            coreDataContext.Files
                .IgnoreQueryFilters()
                .Where(predicate: file => folderIds.Contains(file.FolderId))
                .Select(selector: file => file.Id);

        await coreDataContext.FileContents
            .IgnoreQueryFilters()
            .Where(predicate: fileContent => fileIds.Contains(fileContent.FileId))
            .ExecuteDeleteAsync();

        await coreDataContext.Files
            .IgnoreQueryFilters()
            .Where(predicate: file => folderIds.Contains(file.FolderId))
            .ExecuteDeleteAsync();

        await coreDataContext.FolderRoles
            .IgnoreQueryFilters()
            .Where(predicate: folderRole => folderIds.Contains(folderRole.FolderId))
            .ExecuteDeleteAsync();

        await coreDataContext.Folders
            .IgnoreQueryFilters()
            .Where(predicate: folder => folder.AppId == appId)
            .ExecuteDeleteAsync();
    }

    public int? GetAppId(Folder entity)
    {
        return entity.AppId;
    }
}