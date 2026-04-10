using cCoder.Data;
using cCoder.Data.Models.DMS;
using Microsoft.EntityFrameworkCore;


namespace cCoder.DocumentManagement.Brokers.Storage;

public interface IFileContentBroker
{
    IQueryable<FileContent> GetAllFileContents(bool ignoreFilters);
    ValueTask DeleteAllFileContentsForFileAsync(Guid fileId);
    ValueTask DeleteAllFileContentsForFilesAsync(Guid[] fileIds);
    ValueTask<FileContent> AddFileContentAsync(FileContent entity);
    ValueTask<FileContent> UpdateFileContentAsync(FileContent entity);
    ValueTask<int> DeleteFileContentAsync(FileContent entity);
    ValueTask DeleteAllFileContentsAsync(IEnumerable<FileContent> items);
    int? GetAppId(FileContent entity);
}

public class FileContentBroker(ICoreContextFactory coreContextFactory) : IFileContentBroker
{

    public IQueryable<FileContent> GetAllFileContents(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        return ignoreFilters
            ? coreDataContext.FileContents.IgnoreQueryFilters()
            : coreDataContext.FileContents;
    }

    public async ValueTask<FileContent> AddFileContentAsync(FileContent entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        FileContent result = (await coreDataContext.FileContents.AddAsync(entity)).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<FileContent> UpdateFileContentAsync(FileContent entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        FileContent result = coreDataContext.FileContents.Update(entity).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteFileContentAsync(FileContent entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.FileContents.Remove(entity);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllFileContentsAsync(IEnumerable<FileContent> items)
    {
        if (items == null || !items.Any())
            return;

        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.FileContents.RemoveRange(items);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllFileContentsForFileAsync(Guid fileId)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        List<FileContent> items = coreDataContext.FileContents
            .IgnoreQueryFilters()
            .Where(fileContent => fileContent.FileId == fileId)
            .ToList();

        if (items.Count == 0)
            return;

        coreDataContext.FileContents.RemoveRange(items);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllFileContentsForFilesAsync(Guid[] fileIds)
    {
        if (fileIds == null || fileIds.Length == 0)
            return;

        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        List<FileContent> items = coreDataContext.FileContents
            .IgnoreQueryFilters()
            .Where(fileContent => fileIds.Contains(fileContent.FileId))
            .ToList();

        if (items.Count == 0)
            return;

        coreDataContext.FileContents.RemoveRange(items);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public int? GetAppId(FileContent entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        return coreDataContext.Files

            .Where(file => file.Id == entity.FileId)
            .Join(coreDataContext.Folders,
                file => file.FolderId,
                folder => folder.Id,
                (file, folder) => (int?)folder.AppId)
            .FirstOrDefault();

    }
}







