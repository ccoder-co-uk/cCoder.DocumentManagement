// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.DocumentManagement.Dependencies;
using cCoder.Data.Models.DMS;
using Microsoft.EntityFrameworkCore;


namespace cCoder.DocumentManagement.Brokers.Storage;

public interface IFileContentBroker
{
    IQueryable<FileContent> SelectAllFileContents(bool ignoreFilters);
    ValueTask DeleteAllFileContentsForFileAsync(Guid fileId);
    ValueTask DeleteAllFileContentsForFilesAsync(Guid[] fileIds);
    ValueTask<FileContent> InsertFileContentAsync(FileContent newFileContent);
    ValueTask<FileContent> UpdateFileContentAsync(FileContent updatedFileContent);
    ValueTask<int> DeleteFileContentAsync(FileContent deletedFileContent);
    ValueTask DeleteAllFileContentsAsync(IEnumerable<FileContent> deletedFileContent);
    int? SelectAppId(FileContent entity);
}

internal sealed class FileContentBroker(ICoreContextFactory coreContextFactory) : IFileContentBroker
{

    public IQueryable<FileContent> SelectAllFileContents(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return Branching.ApplyQueryFilters(query: coreDataContext.FileContents, ignoreFilters: ignoreFilters);
    }

    public async ValueTask<FileContent> InsertFileContentAsync(FileContent newFileContent)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        FileContent result = (await coreDataContext.FileContents.AddAsync(entity: newFileContent)).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<FileContent> UpdateFileContentAsync(FileContent updatedFileContent)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        FileContent result = coreDataContext.FileContents.Update(entity: updatedFileContent).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteFileContentAsync(FileContent deletedFileContent)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.FileContents.Remove(entity: deletedFileContent);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllFileContentsAsync(IEnumerable<FileContent> deletedFileContent)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.FileContents.RemoveRange(entities: deletedFileContent ?? []);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllFileContentsForFileAsync(Guid fileId)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        List<FileContent> items = coreDataContext.FileContents
            .IgnoreQueryFilters()
            .Where(predicate: fileContent => fileContent.FileId == fileId)
            .ToList();

        coreDataContext.FileContents.RemoveRange(entities: items);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllFileContentsForFilesAsync(Guid[] fileIds)
    {
        fileIds ??= [];

        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        List<FileContent> items = coreDataContext.FileContents
            .IgnoreQueryFilters()
            .Where(predicate: fileContent => fileIds.Contains(value: fileContent.FileId))
            .ToList();

        coreDataContext.FileContents.RemoveRange(entities: items);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public int? SelectAppId(FileContent entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.Files

            .Where(predicate: file => file.Id == entity.FileId)
            .Join(inner: coreDataContext.Folders,
                outerKeySelector: file => file.FolderId,
                innerKeySelector: folder => folder.Id,
                resultSelector: (file, folder) => (int?)folder.AppId)
            .FirstOrDefault();

    }
}
