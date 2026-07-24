// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.DocumentManagement.Brokers.Storage;
using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using IAuthorizationBroker = cCoder.DocumentManagement.Brokers.IAuthorizationBroker;
using LocalFile = cCoder.Data.Models.DMS.File;


namespace cCoder.DocumentManagement.Services.Foundations;

internal partial class FileContentService(
    IFileContentBroker fileContentBroker,
    IAuthorizationBroker authorizationBroker
) : IFileContentService
{
    public FileContent Get(Guid id)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [id]);
            FileContent fileContent = GetAll()
    .FirstOrDefault(predicate: i => i.Id == id);


            if (fileContent is not null)
            {
                return fileContent;
            }


            FileContent unrestrictedFileContent = GetAll(ignoreFilters: true)
                .FirstOrDefault(predicate: i => i.Id == id);


            if (unrestrictedFileContent is not null)
            {
                throw new SecurityException(message: "Access Denied!");
            }


            return null;

        });

    public IQueryable<FileContent> GetAll(bool ignoreFilters = false)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [ignoreFilters]);
            return fileContentBroker.SelectAllFileContents(ignoreFilters: ignoreFilters);
        });

    public ValueTask DeleteAllForFileAsync(Guid fileId)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [fileId]);
            return fileContentBroker.DeleteAllFileContentsForFileAsync(fileId: fileId);
        });

    public ValueTask DeleteAllForFilesAsync(Guid[] fileIds)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [fileIds]);
            return fileContentBroker.DeleteAllFileContentsForFilesAsync(fileIds: fileIds);
        });

    public ValueTask<FileContent> AddFileContentAsync(FileContent fileContent)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [fileContent]);
            cCoder.Data.Models.DMS.FileContent newFileContent = CreateStorageFileContent(fileContent: fileContent, includeId: false);


            authorizationBroker.Authorize(
                appId: fileContentBroker.GetAppId(entity: newFileContent),
                privilege: $"{nameof(FileContent)}_create"
            );


            string currentUserId = authorizationBroker.GetCurrentUser().Id;

            DateTimeOffset now = DateTimeOffset.UtcNow;

            newFileContent.CreatedOn = now;

            newFileContent.CreatedBy = currentUserId;


            FileContent result = await fileContentBroker.InsertFileContentAsync(entity: newFileContent);

            fileContent.Id = result.Id;

            fileContent.FileId = result.FileId;

            fileContent.Description = result.Description;

            fileContent.Size = result.Size;

            fileContent.CreatedBy = result.CreatedBy;

            fileContent.CreatedOn = result.CreatedOn;

            fileContent.Version = result.Version;

            fileContent.RawData = result.RawData;

            return fileContent;

        });

    public ValueTask<FileContent> UpdateFileContentAsync(FileContent fileContent)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [fileContent]);
            cCoder.Data.Models.DMS.FileContent updateFileContent = CreateStorageFileContent(fileContent: fileContent, includeId: true);


            authorizationBroker.Authorize(
                appId: fileContentBroker.GetAppId(entity: updateFileContent),
                privilege: $"{nameof(FileContent)}_update"
            );


            FileContent result = await fileContentBroker.UpdateFileContentAsync(entity: updateFileContent);

            fileContent.Id = result.Id;

            fileContent.FileId = result.FileId;

            fileContent.Description = result.Description;

            fileContent.Size = result.Size;

            fileContent.CreatedBy = result.CreatedBy;

            fileContent.CreatedOn = result.CreatedOn;

            fileContent.Version = result.Version;

            fileContent.RawData = result.RawData;

            return fileContent;

        });

    public ValueTask DeleteAsync(Guid id)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [id]);
            FileContent fileContent = Get(id: id);


            authorizationBroker.Authorize(
                appId: fileContentBroker.GetAppId(entity: CreateStorageFileContent(fileContent: fileContent, includeId: true)),
                privilege: $"{nameof(FileContent)}_delete"
            );


            _ = await fileContentBroker.DeleteFileContentAsync(entity: CreateStorageFileContent(fileContent: fileContent, includeId: true));

        });

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