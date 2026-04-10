using cCoder.Data;
using Microsoft.EntityFrameworkCore;
using FileEntity = cCoder.Data.Models.DMS.File;


namespace cCoder.DocumentManagement.Brokers.Storage;

public interface IFileBroker
{
    IQueryable<FileEntity> GetAllFiles(bool ignoreFilters);
    Guid[] GetFileIdsByFolderIds(Guid[] folderIds, bool ignoreFilters);
    FileEntity GetFileByPath(int appId, string path, bool ignoreFilters);
    FileEntity GetFileByPathWithFolderAndContents(int appId, string path, bool ignoreFilters);
    FileEntity GetFileByPathWithFolderRolesAndContents(int appId, string path, bool ignoreFilters);
    FileEntity GetFileWithFolderAndContents(Guid id, bool ignoreFilters);
    FileEntity GetFileWithFolderRolesAndContents(Guid id, bool ignoreFilters);
    IQueryable<FileEntity> SearchFiles(int appId, byte[] needle);
    ValueTask<FileEntity> AddFileAsync(FileEntity entity);
    ValueTask<FileEntity> UpdateFileAsync(FileEntity entity);
    ValueTask<int> DeleteFileAsync(FileEntity entity);
    ValueTask DeleteAllFilesAsync(IEnumerable<FileEntity> items);
    int? GetAppId(FileEntity entity);
}

public class FileBroker(ICoreContextFactory coreContextFactory) : IFileBroker
{

    public IQueryable<FileEntity> GetAllFiles(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        return ignoreFilters
            ? coreDataContext.Files.IgnoreQueryFilters()
            : coreDataContext.Files;
    }

    public FileEntity GetFileByPath(int appId, string path, bool ignoreFilters)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        IQueryable<FileEntity> query = ignoreFilters
            ? coreDataContext.Files.IgnoreQueryFilters()
            : coreDataContext.Files;

        return query
            .Include(file => file.Folder)
            .FirstOrDefault(file => file.Folder.AppId == appId && file.Path.ToLower() == path.ToLower());
    }

    public Guid[] GetFileIdsByFolderIds(Guid[] folderIds, bool ignoreFilters)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        IQueryable<FileEntity> query = ignoreFilters
            ? coreDataContext.Files.IgnoreQueryFilters()
            : coreDataContext.Files;

        return query
            .Where(file => folderIds.Contains(file.FolderId))
            .Select(file => file.Id)
            .ToArray();
    }

    public FileEntity GetFileByPathWithFolderAndContents(int appId, string path, bool ignoreFilters)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        IQueryable<FileEntity> query = ignoreFilters
            ? coreDataContext.Files.IgnoreQueryFilters()
            : coreDataContext.Files;

        return query
            .Include(file => file.Folder)
            .Include(file => file.Contents)
            .FirstOrDefault(file => file.Folder.AppId == appId && file.Path.ToLower() == path.ToLower());
    }

    public FileEntity GetFileByPathWithFolderRolesAndContents(int appId, string path, bool ignoreFilters)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        IQueryable<FileEntity> query = ignoreFilters
            ? coreDataContext.Files.IgnoreQueryFilters()
            : coreDataContext.Files;

        return query
            .Include(file => file.Contents)
            .Include(file => file.Folder)
                .ThenInclude(folder => folder.Roles)
                    .ThenInclude(folderRole => folderRole.Role)
            .FirstOrDefault(file => file.Folder.AppId == appId && file.Path.ToLower() == path.ToLower());
    }

    public FileEntity GetFileWithFolderAndContents(Guid id, bool ignoreFilters)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        IQueryable<FileEntity> query = ignoreFilters
            ? coreDataContext.Files.IgnoreQueryFilters()
            : coreDataContext.Files;

        return query
            .Include(file => file.Folder)
            .Include(file => file.Contents)
            .FirstOrDefault(file => file.Id == id);
    }

    public FileEntity GetFileWithFolderRolesAndContents(Guid id, bool ignoreFilters)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        IQueryable<FileEntity> query = ignoreFilters
            ? coreDataContext.Files.IgnoreQueryFilters()
            : coreDataContext.Files;

        return query
            .Include(file => file.Contents)
            .Include(file => file.Folder)
                .ThenInclude(folder => folder.Roles)
                    .ThenInclude(folderRole => folderRole.Role)
            .FirstOrDefault(file => file.Id == id);
    }

    public IQueryable<FileEntity> SearchFiles(int appId, byte[] needle)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        return coreDataContext.Files
            .Include(file => file.Folder)
            .Include(file => file.Contents)
            .Where(file =>
                file.Folder.AppId == appId
                && file.Contents.Any(content => content.RawData.SequenceEqual(needle)));
    }

    public async ValueTask<FileEntity> AddFileAsync(FileEntity entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        FileEntity result = (await coreDataContext.Files.AddAsync(entity)).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<FileEntity> UpdateFileAsync(FileEntity entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        FileEntity result = coreDataContext.Files.Update(entity).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteFileAsync(FileEntity entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Files.Remove(entity);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllFilesAsync(IEnumerable<FileEntity> items)
    {
        if (items == null || !items.Any())
            return;

        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Files.RemoveRange(items);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public int? GetAppId(FileEntity entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        return coreDataContext.Folders

            .Where(folder => folder.Id == entity.FolderId)
            .Select(folder => (int?)folder.AppId)
            .FirstOrDefault();

    }
}






