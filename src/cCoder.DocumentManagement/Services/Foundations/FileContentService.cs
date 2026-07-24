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
    public FileContent Get(Guid fileContentId)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [fileContentId]);
            FileContent fileContent = GetAllValue()
    .FirstOrDefault(predicate: i => i.Id == fileContentId);


            if (fileContent is not null)
            {
                return fileContent;
            }


            FileContent unrestrictedFileContent = GetAllValue(ignoreFilters: true)
                .FirstOrDefault(predicate: i => i.Id == fileContentId);


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

    public ValueTask<FileContent> AddFileContentAsync(FileContent newFileContent)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [newFileContent]);
            cCoder.Data.Models.DMS.FileContent storageFileContent =
                CreateLocalFileEntityContent(fileContent: newFileContent, includeId: false);


            authorizationBroker.Authorize(
                appId: fileContentBroker.SelectAppId(entity: storageFileContent),
                privilege: $"{nameof(FileContent)}_create"
            );


            string currentUserId = authorizationBroker.GetCurrentUser().Id;

            DateTimeOffset now = DateTimeOffset.UtcNow;

            storageFileContent.CreatedOn = now;

            storageFileContent.CreatedBy = currentUserId;


            FileContent result = await fileContentBroker.InsertFileContentAsync(newFileContent: storageFileContent);

            newFileContent.Id = result.Id;

            newFileContent.FileId = result.FileId;

            newFileContent.Description = result.Description;

            newFileContent.Size = result.Size;

            newFileContent.CreatedBy = result.CreatedBy;

            newFileContent.CreatedOn = result.CreatedOn;

            newFileContent.Version = result.Version;

            newFileContent.RawData = result.RawData;

            return newFileContent;

        });

    public ValueTask<FileContent> UpdateFileContentAsync(FileContent updatedFileContent)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [updatedFileContent]);
            cCoder.Data.Models.DMS.FileContent updateFileContent = CreateLocalFileEntityContent(fileContent: updatedFileContent, includeId: true);


            authorizationBroker.Authorize(
                appId: fileContentBroker.SelectAppId(entity: updateFileContent),
                privilege: $"{nameof(FileContent)}_update"
            );


            FileContent result = await fileContentBroker.UpdateFileContentAsync(updatedFileContent: updateFileContent);

            updatedFileContent.Id = result.Id;

            updatedFileContent.FileId = result.FileId;

            updatedFileContent.Description = result.Description;

            updatedFileContent.Size = result.Size;

            updatedFileContent.CreatedBy = result.CreatedBy;

            updatedFileContent.CreatedOn = result.CreatedOn;

            updatedFileContent.Version = result.Version;

            updatedFileContent.RawData = result.RawData;

            return updatedFileContent;

        });

    public ValueTask DeleteAsync(Guid fileContentId)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [fileContentId]);
            FileContent fileContent = GetValue(fileContentId: fileContentId);


            authorizationBroker.Authorize(
                appId: fileContentBroker.SelectAppId(entity: CreateLocalFileEntityContent(fileContent: fileContent, includeId: true)),
                privilege: $"{nameof(FileContent)}_delete"
            );


            _ = await fileContentBroker.DeleteFileContentAsync(deletedFileContent: CreateLocalFileEntityContent(fileContent: fileContent, includeId: true));

        });

    private static cCoder.Data.Models.DMS.FileContent CreateLocalFileEntityContent(FileContent fileContent, bool includeId)
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

    private FileContent GetValue(Guid fileContentId) =>
        Get(fileContentId: fileContentId);

    private IQueryable<FileContent> GetAllValue(bool ignoreFilters = false) =>
        GetAll(ignoreFilters: ignoreFilters);
}