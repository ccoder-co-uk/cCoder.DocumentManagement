using System.Security;
using cCoder.DocumentManagement.Brokers.Storage;
using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using IAuthorizationBroker = cCoder.DocumentManagement.Brokers.IAuthorizationBroker;
using LocalFile = cCoder.Data.Models.DMS.File;


namespace cCoder.DocumentManagement.Services.Foundations;

internal class FileContentService(
    IFileContentBroker fileContentBroker,
    IAuthorizationBroker authorizationBroker
) : IFileContentService
{
    public FileContent Get(Guid id)
    {
        FileContent fileContent = GetAll().FirstOrDefault(i => i.Id == id);
        if (fileContent is not null)
            return fileContent;

        FileContent unrestrictedFileContent = GetAll(true).FirstOrDefault(i => i.Id == id);
        if (unrestrictedFileContent is not null)
            throw new SecurityException("Access Denied!");

        return null;
    }

    public IQueryable<FileContent> GetAll(bool ignoreFilters = false) =>
        fileContentBroker.GetAllFileContents(ignoreFilters);

    public ValueTask DeleteAllForFileAsync(Guid fileId) =>
        fileContentBroker.DeleteAllFileContentsForFileAsync(fileId);

    public ValueTask DeleteAllForFilesAsync(Guid[] fileIds) =>
        fileContentBroker.DeleteAllFileContentsForFilesAsync(fileIds);

    public async ValueTask<FileContent> AddAsync(FileContent fileContent)
    {
        cCoder.Data.Models.DMS.FileContent newFileContent = CreateStorageFileContent(fileContent, includeId: false);
        authorizationBroker.Authorize(
            fileContentBroker.GetAppId(newFileContent),
            $"{nameof(FileContent)}_create"
        );
        string currentUserId = authorizationBroker.GetCurrentUser().Id;
        DateTimeOffset now = DateTimeOffset.UtcNow;
        newFileContent.CreatedOn = now;
        newFileContent.CreatedBy = currentUserId;

        FileContent result = await fileContentBroker.AddFileContentAsync(newFileContent);
        fileContent.Id = result.Id;
        fileContent.FileId = result.FileId;
        fileContent.Description = result.Description;
        fileContent.Size = result.Size;
        fileContent.CreatedBy = result.CreatedBy;
        fileContent.CreatedOn = result.CreatedOn;
        fileContent.Version = result.Version;
        fileContent.RawData = result.RawData;
        return fileContent;
    }

    public async ValueTask<FileContent> UpdateAsync(FileContent fileContent)
    {
        cCoder.Data.Models.DMS.FileContent updateFileContent = CreateStorageFileContent(fileContent, includeId: true);
        authorizationBroker.Authorize(
            fileContentBroker.GetAppId(updateFileContent),
            $"{nameof(FileContent)}_update"
        );

        FileContent result = await fileContentBroker.UpdateFileContentAsync(updateFileContent);
        fileContent.Id = result.Id;
        fileContent.FileId = result.FileId;
        fileContent.Description = result.Description;
        fileContent.Size = result.Size;
        fileContent.CreatedBy = result.CreatedBy;
        fileContent.CreatedOn = result.CreatedOn;
        fileContent.Version = result.Version;
        fileContent.RawData = result.RawData;
        return fileContent;
    }

    public async ValueTask DeleteAsync(Guid id)
    {
        FileContent fileContent = Get(id);
        authorizationBroker.Authorize(
            fileContentBroker.GetAppId(CreateStorageFileContent(fileContent, includeId: true)),
            $"{nameof(FileContent)}_delete"
        );
        _ = await fileContentBroker.DeleteFileContentAsync(CreateStorageFileContent(fileContent, includeId: true));
    }

    private static cCoder.Data.Models.DMS.FileContent CreateStorageFileContent(FileContent fileContent, bool includeId)
    {
        if (fileContent == null)
        {
            return null;
        }

        return new cCoder.Data.Models.DMS.FileContent
        {
            Id = includeId ? fileContent.Id : Guid.Empty,
            FileId = fileContent.FileId,
            Description = fileContent.Description,
            Size = fileContent.Size,
            CreatedBy = fileContent.CreatedBy,
            CreatedOn = fileContent.CreatedOn,
            Version = fileContent.Version,
            RawData = fileContent.RawData
        };
    }
}












