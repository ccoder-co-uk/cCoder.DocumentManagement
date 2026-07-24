// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.DocumentManagement.Dependencies;
using Microsoft.EntityFrameworkCore;
using FileEntity = cCoder.Data.Models.DMS.File;


namespace cCoder.DocumentManagement.Brokers.Storage;

public interface IFileBroker
{
    IQueryable<FileEntity> SelectAllFiles(bool ignoreFilters);
    Guid[] SelectFileIdsByFolderIds(Guid[] folderIds, bool ignoreFilters);
    FileEntity SelectFileByPath(int appId, string path, bool ignoreFilters);
    FileEntity SelectFileByPathWithFolderAndContents(int appId, string path, bool ignoreFilters);
    FileEntity SelectFileByPathWithFolderRolesAndContents(int appId, string path, bool ignoreFilters);
    FileEntity SelectFileWithFolderAndContents(Guid fileId, bool ignoreFilters);
    FileEntity SelectFileWithFolderRolesAndContents(Guid fileId, bool ignoreFilters);
    IQueryable<FileEntity> SelectFilesByContent(int appId, byte[] needle);
    ValueTask<FileEntity> InsertFileAsync(FileEntity entity);
    ValueTask<FileEntity> UpdateFileAsync(FileEntity entity);
    ValueTask<int> DeleteFileAsync(FileEntity entity);
    ValueTask DeleteAllFilesAsync(IEnumerable<FileEntity> items);
    int? SelectAppId(FileEntity entity);
}

internal sealed class FileBroker(ICoreContextFactory coreContextFactory) : IFileBroker
{

    public IQueryable<FileEntity> SelectAllFiles(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return Branching.ApplyQueryFilters(query: coreDataContext.Files, ignoreFilters: ignoreFilters);
    }

    public FileEntity SelectFileByPath(int appId, string path, bool ignoreFilters)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        IQueryable<FileEntity> query = Branching.ApplyQueryFilters(query: coreDataContext.Files, ignoreFilters: ignoreFilters);

        return query
            .Include(navigationPropertyPath: file => file.Folder)
            .FirstOrDefault(predicate: file => file.Folder.AppId == appId && file.Path.ToLower() == path.ToLower());
    }

    public Guid[] SelectFileIdsByFolderIds(Guid[] folderIds, bool ignoreFilters)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        IQueryable<FileEntity> query = Branching.ApplyQueryFilters(query: coreDataContext.Files, ignoreFilters: ignoreFilters);

        return query
            .Where(predicate: file => folderIds.Contains(value: file.FolderId))
            .Select(selector: file => file.Id)
            .ToArray();
    }

    public FileEntity SelectFileByPathWithFolderAndContents(int appId, string path, bool ignoreFilters)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        IQueryable<FileEntity> query = Branching.ApplyQueryFilters(query: coreDataContext.Files, ignoreFilters: ignoreFilters);

        return query
            .Include(navigationPropertyPath: file => file.Folder)
            .Include(navigationPropertyPath: file => file.Contents)
            .FirstOrDefault(predicate: file => file.Folder.AppId == appId && file.Path.ToLower() == path.ToLower());
    }

    public FileEntity SelectFileByPathWithFolderRolesAndContents(int appId, string path, bool ignoreFilters)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        IQueryable<FileEntity> query = Branching.ApplyQueryFilters(query: coreDataContext.Files, ignoreFilters: ignoreFilters);

        return query
            .Include(navigationPropertyPath: file => file.Contents)
            .Include(navigationPropertyPath: file => file.Folder)
                .ThenInclude(navigationPropertyPath: folder => folder.Roles)
                    .ThenInclude(navigationPropertyPath: folderRole => folderRole.Role)
            .FirstOrDefault(predicate: file => file.Folder.AppId == appId && file.Path.ToLower() == path.ToLower());
    }

    public FileEntity SelectFileWithFolderAndContents(Guid fileId, bool ignoreFilters)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        IQueryable<FileEntity> query = Branching.ApplyQueryFilters(query: coreDataContext.Files, ignoreFilters: ignoreFilters);

        return query
            .Include(navigationPropertyPath: file => file.Folder)
            .Include(navigationPropertyPath: file => file.Contents)
            .FirstOrDefault(predicate: file => file.Id == fileId);
    }

    public FileEntity SelectFileWithFolderRolesAndContents(Guid fileId, bool ignoreFilters)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        IQueryable<FileEntity> query = Branching.ApplyQueryFilters(query: coreDataContext.Files, ignoreFilters: ignoreFilters);

        return query
            .Include(navigationPropertyPath: file => file.Contents)
            .Include(navigationPropertyPath: file => file.Folder)
                .ThenInclude(navigationPropertyPath: folder => folder.Roles)
                    .ThenInclude(navigationPropertyPath: folderRole => folderRole.Role)
            .FirstOrDefault(predicate: file => file.Id == fileId);
    }

    public IQueryable<FileEntity> SelectFilesByContent(int appId, byte[] needle)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.Files
            .Include(navigationPropertyPath: file => file.Folder)
            .Include(navigationPropertyPath: file => file.Contents)
            .Where(predicate: file =>
                file.Folder.AppId == appId
                && file.Contents.Any(predicate: content => content.RawData.SequenceEqual(other: needle)));
    }

    public async ValueTask<FileEntity> InsertFileAsync(FileEntity entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        FileEntity result = (await coreDataContext.Files.AddAsync(entity: entity)).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<FileEntity> UpdateFileAsync(FileEntity entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        FileEntity result = coreDataContext.Files.Update(entity: entity).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteFileAsync(FileEntity entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Files.Remove(entity: entity);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllFilesAsync(IEnumerable<FileEntity> items)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Files.RemoveRange(entities: items ?? []);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public int? SelectAppId(FileEntity entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.Folders

            .Where(predicate: folder => folder.Id == entity.FolderId)
            .Select(selector: folder => (int?)folder.AppId)
            .FirstOrDefault();

    }
}