// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.DocumentManagement.Dependencies;
using cCoder.Data.Models.DMS;
using Microsoft.EntityFrameworkCore;


namespace cCoder.DocumentManagement.Brokers.Storage;

public interface IFolderBroker
{
    IQueryable<Folder> SelectAllFolders(bool ignoreFilters);
    Folder SelectFolderWithRoles(Guid folderId, bool ignoreFilters);
    Folder SelectFolderForUpdate(Guid folderId, bool ignoreFilters);
    Folder SelectFolderByPath(int appId, string path, bool ignoreFilters);
    Folder SelectFolderByPathWithRoles(int appId, string path, bool ignoreFilters);
    Folder SelectFolderByPathWithParentAndRoles(int appId, string path, bool ignoreFilters);
    Folder SelectFolderByPathWithRolesAndFilesAndContents(int appId, string path, bool ignoreFilters);
    Folder SelectFolderByPathWithSubFoldersAndFiles(int appId, string path, bool ignoreFilters);
    ValueTask<Folder> InsertFolderAsync(Folder newFolder);
    ValueTask<Folder> UpdateFolderAsync(Folder updatedFolder);
    ValueTask<int> DeleteFolderAsync(Folder deletedFolder);
    ValueTask DeleteAllFoldersAsync(IEnumerable<Folder> deletedFolder);
    ValueTask DeleteAllFoldersByAppIdAsync(int appId);
    int? SelectAppId(Folder entity);
}

internal sealed class FolderBroker(ICoreContextFactory coreContextFactory) : IFolderBroker
{

    public IQueryable<Folder> SelectAllFolders(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        IQueryable<Folder> query = Branching.ApplyQueryFilters(query: coreDataContext.Folders, ignoreFilters: ignoreFilters);

        return query
            .Include(navigationPropertyPath: folder => folder.Files)
            .Include(navigationPropertyPath: folder => folder.SubFolders)
            .AsSplitQuery();
    }

    public Folder SelectFolderWithRoles(Guid folderId, bool ignoreFilters)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        IQueryable<Folder> query = Branching.ApplyQueryFilters(query: coreDataContext.Folders, ignoreFilters: ignoreFilters);

        return query
            .Include(navigationPropertyPath: folder => folder.Roles)
                .ThenInclude(navigationPropertyPath: folderRole => folderRole.Role)
            .FirstOrDefault(predicate: folder => folder.Id == folderId);
    }

    public Folder SelectFolderForUpdate(Guid folderId, bool ignoreFilters)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        IQueryable<Folder> query = Branching.ApplyQueryFilters(query: coreDataContext.Folders, ignoreFilters: ignoreFilters);

        return query
            .Include(navigationPropertyPath: folder => folder.App)
            .Include(navigationPropertyPath: folder => folder.SubFolders)
            .Include(navigationPropertyPath: folder => folder.Parent)
            .Include(navigationPropertyPath: folder => folder.Files)
            .Include(navigationPropertyPath: folder => folder.Roles)
                .ThenInclude(navigationPropertyPath: folderRole => folderRole.Role)
            .AsSplitQuery()
            .FirstOrDefault(predicate: folder => folder.Id == folderId);
    }

    public Folder SelectFolderByPath(int appId, string path, bool ignoreFilters)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        IQueryable<Folder> query = Branching.ApplyQueryFilters(query: coreDataContext.Folders, ignoreFilters: ignoreFilters);

        return query.FirstOrDefault(predicate: folder => folder.AppId == appId && folder.Path == path);
    }

    public Folder SelectFolderByPathWithRoles(int appId, string path, bool ignoreFilters)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        IQueryable<Folder> query = Branching.ApplyQueryFilters(query: coreDataContext.Folders, ignoreFilters: ignoreFilters);

        return query
            .Include(navigationPropertyPath: folder => folder.Roles)
                .ThenInclude(navigationPropertyPath: folderRole => folderRole.Role)
            .FirstOrDefault(predicate: folder => folder.AppId == appId && folder.Path == path);
    }

    public Folder SelectFolderByPathWithParentAndRoles(int appId, string path, bool ignoreFilters)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        IQueryable<Folder> query = Branching.ApplyQueryFilters(query: coreDataContext.Folders, ignoreFilters: ignoreFilters);

        return query
            .Include(navigationPropertyPath: folder => folder.Parent)
            .Include(navigationPropertyPath: folder => folder.Roles)
                .ThenInclude(navigationPropertyPath: folderRole => folderRole.Role)
            .FirstOrDefault(predicate: folder => folder.AppId == appId && folder.Path == path);
    }

    public Folder SelectFolderByPathWithRolesAndFilesAndContents(int appId, string path, bool ignoreFilters)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        IQueryable<Folder> query = Branching.ApplyQueryFilters(query: coreDataContext.Folders, ignoreFilters: ignoreFilters);

        return query
            .Include(navigationPropertyPath: folder => folder.Roles)
                .ThenInclude(navigationPropertyPath: folderRole => folderRole.Role)
            .Include(navigationPropertyPath: folder => folder.Files)
                .ThenInclude(navigationPropertyPath: file => file.Contents)
            .FirstOrDefault(predicate: folder => folder.AppId == appId && folder.Path == path);
    }

    public Folder SelectFolderByPathWithSubFoldersAndFiles(int appId, string path, bool ignoreFilters)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        IQueryable<Folder> query = Branching.ApplyQueryFilters(query: coreDataContext.Folders, ignoreFilters: ignoreFilters);

        return query
            .Include(navigationPropertyPath: folder => folder.SubFolders)
            .Include(navigationPropertyPath: folder => folder.Files)
            .FirstOrDefault(predicate: folder => folder.AppId == appId && folder.Path == path);
    }

    public async ValueTask<Folder> InsertFolderAsync(Folder newFolder)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Folder result = (await coreDataContext.Folders.AddAsync(entity: newFolder)).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<Folder> UpdateFolderAsync(Folder updatedFolder)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Folder result = coreDataContext.Folders.Update(entity: updatedFolder).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteFolderAsync(Folder deletedFolder)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        Folder folder = await coreDataContext.Folders
            .IgnoreQueryFilters()
            .Include(navigationPropertyPath: foundFolder => foundFolder.Roles)
            .FirstOrDefaultAsync(predicate: foundFolder => foundFolder.Id == deletedFolder.Id);

        return await Branching.ExecuteWhenNotNullAsync(
            input: folder,
            operation: async foundFolder =>
            {
                coreDataContext.FolderRoles.RemoveRange(
                    entities: foundFolder.Roles ?? []);

                coreDataContext.Folders.Remove(entity: foundFolder);

                return await coreDataContext.SaveChangesAsync();
            },
            defaultValue: 0);
    }

    public async ValueTask DeleteAllFoldersAsync(IEnumerable<Folder> deletedFolder)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Guid[] folderIds =
        [
            .. (deletedFolder ?? [])
                .Select(selector: folder => folder.Id),
        ];

        Folder[] folders = await coreDataContext.Folders
            .IgnoreQueryFilters()
            .Include(navigationPropertyPath: folder => folder.Roles)
            .Where(predicate: folder => folderIds.Contains(value: folder.Id))
            .ToArrayAsync();

        coreDataContext.FolderRoles.RemoveRange(
            entities: folders.SelectMany(selector: folder => folder.Roles ?? []));

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
                .Where(predicate: file => folderIds.Contains(item: file.FolderId))
                .Select(selector: file => file.Id);

        await coreDataContext.FileContents
            .IgnoreQueryFilters()
            .Where(predicate: fileContent => fileIds.Contains(item: fileContent.FileId))
            .ExecuteDeleteAsync();

        await coreDataContext.Files
            .IgnoreQueryFilters()
            .Where(predicate: file => folderIds.Contains(item: file.FolderId))
            .ExecuteDeleteAsync();

        await coreDataContext.FolderRoles
            .IgnoreQueryFilters()
            .Where(predicate: folderRole => folderIds.Contains(item: folderRole.FolderId))
            .ExecuteDeleteAsync();

        await coreDataContext.Folders
            .IgnoreQueryFilters()
            .Where(predicate: folder => folder.AppId == appId)
            .ExecuteDeleteAsync();
    }

    public int? SelectAppId(Folder entity)
    {
        return entity.AppId;
    }
}
