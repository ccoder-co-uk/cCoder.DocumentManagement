// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.DocumentManagement.Brokers.Storage;
using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using FileEntity = cCoder.Data.Models.DMS.File;
using IAuthorizationBroker = cCoder.DocumentManagement.Brokers.IAuthorizationBroker;
using LocalFile = cCoder.Data.Models.DMS.File;


namespace cCoder.DocumentManagement.Services.Foundations;

internal partial class FileService(IFileBroker fileBroker, IAuthorizationBroker authorizationBroker)
    : IFileService
{
    public LocalFile Get(Guid fileId)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [fileId]);
            LocalFile file = GetAllValue()
    .FirstOrDefault(predicate: i => i.Id == fileId);


            if (file is not null)
            {
                return file;
            }


            LocalFile unrestrictedFile = GetAllValue(ignoreFilters: true)
                .FirstOrDefault(predicate: i => i.Id == fileId);


            if (unrestrictedFile is not null)
            {
                throw new SecurityException(message: "Access Denied!");
            }


            return null;

        });

    public IQueryable<LocalFile> GetAll(bool ignoreFilters = false)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [ignoreFilters]);
            return fileBroker.SelectAllFiles(ignoreFilters: ignoreFilters);
        });

    public Guid[] GetIdsByFolderIds(Guid[] folderIds, bool ignoreFilters = false)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [folderIds, ignoreFilters]);
            return fileBroker.SelectFileIdsByFolderIds(folderIds: folderIds, ignoreFilters: ignoreFilters);
        });

    public LocalFile GetWithFolderAndContents(Guid fileId, bool ignoreFilters = false)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [fileId, ignoreFilters]);
            return CreateLocalFile(file: fileBroker.SelectFileWithFolderAndContents(fileId: fileId, ignoreFilters: ignoreFilters));
        });

    public LocalFile GetWithFolderRolesAndContents(Guid fileId, bool ignoreFilters = false)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [fileId, ignoreFilters]);
            return CreateLocalFileWithFolderRoles(file: fileBroker.SelectFileWithFolderRolesAndContents(fileId: fileId, ignoreFilters: ignoreFilters));
        });

    public LocalFile GetByPath(int appId, string path, bool ignoreFilters = false)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [appId, path, ignoreFilters]);
            return CreateLocalFile(file: fileBroker.SelectFileByPath(appId: appId, path: path, ignoreFilters: ignoreFilters));
        });

    public LocalFile GetByPathWithFolderAndContents(
        int appId,
        string path,
        bool ignoreFilters = false
    )
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [appId, path, ignoreFilters]);
            return CreateLocalFile(file: fileBroker.SelectFileByPathWithFolderAndContents(appId: appId, path: path, ignoreFilters: ignoreFilters));
        });

    public LocalFile GetByPathWithFolderRolesAndContents(
        int appId,
        string path,
        bool ignoreFilters = false
    )
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [appId, path, ignoreFilters]);
            return CreateLocalFileWithFolderRoles(
                file: fileBroker.SelectFileByPathWithFolderRolesAndContents(appId: appId, path: path, ignoreFilters: ignoreFilters)
            );
        });

    public IQueryable<LocalFile> Search(int appId, byte[] needle)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [appId, needle]);
            return fileBroker.SelectFilesByContent(appId: appId, needle: needle)
                                                                .AsEnumerable()
                                                                               .Select(selector: CreateLocalFile)
                                                                                                            .AsQueryable();
        });

    public ValueTask<LocalFile> AddFileAsync(LocalFile file)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [file]);
            FileEntity newFileEntity = CreateLocalFileEntity(file: file, includeId: false);


            authorizationBroker.Authorize(
                appId: fileBroker.SelectAppId(entity: newFileEntity),
                privilege: $"{nameof(cCoder.Data.Models.DMS.File)}_create"
            );


            string currentUserId = authorizationBroker.GetCurrentUser().Id;

            DateTimeOffset now = DateTimeOffset.UtcNow;

            newFileEntity.CreatedOn = now;

            newFileEntity.CreatedBy = currentUserId;


            FileEntity result = await fileBroker.InsertFileAsync(entity: newFileEntity);

            file.Id = result.Id;

            file.FolderId = result.FolderId;

            file.Name = result.Name;

            file.Description = result.Description;

            file.Path = result.Path;

            file.MimeType = result.MimeType;

            file.CreatedBy = result.CreatedBy;

            file.Size = result.Size;

            file.CreatedOn = result.CreatedOn;

            file.DeletedOn = result.DeletedOn;

            return file;

        });

    public ValueTask<LocalFile> UpdateFileAsync(LocalFile file)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [file]);
            FileEntity updateFileEntity = CreateLocalFileEntity(file: file, includeId: true);


            authorizationBroker.Authorize(
                appId: fileBroker.SelectAppId(entity: updateFileEntity),
                privilege: $"{nameof(cCoder.Data.Models.DMS.File)}_update"
            );


            return await UpdateForAppFileValueAsync(file: file);

        });

    public ValueTask<LocalFile> UpdateForAppFileAsync(LocalFile file)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [file]);
            FileEntity updateFileEntity = CreateLocalFileEntity(file: file, includeId: true);

            FileEntity result = await fileBroker.UpdateFileAsync(entity: updateFileEntity);

            file.Id = result.Id;

            file.FolderId = result.FolderId;

            file.Name = result.Name;

            file.Description = result.Description;

            file.Path = result.Path;

            file.MimeType = result.MimeType;

            file.CreatedBy = result.CreatedBy;

            file.Size = result.Size;

            file.CreatedOn = result.CreatedOn;

            file.DeletedOn = result.DeletedOn;

            return file;

        });

    public ValueTask DeleteAsync(Guid fileId)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [fileId]);
            LocalFile file = GetAllValue(ignoreFilters: true)
    .FirstOrDefault(predicate: foundFile => foundFile.Id == fileId);


            if (file is null)
            {
                return;
            }


            authorizationBroker.Authorize(
                appId: fileBroker.SelectAppId(entity: new FileEntity
                {
                    Id = file.Id,
                    FolderId = file.FolderId,
                }),
                privilege: $"file_delete"
            );


            _ = await fileBroker.DeleteFileAsync(entity: new FileEntity
            {
                Id = file.Id,
                FolderId = file.FolderId,
            });

        });

    public ValueTask DeleteAllForAppFileAsync(IEnumerable<LocalFile> items)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [items]);
            return fileBroker.DeleteAllFilesAsync(
                items: items?.Select(selector: file => CreateLocalFileEntity(file: file, includeId: true)) ?? []);
        });

    private static LocalFile CreateLocalFile(FileEntity file) =>
        file == null
            ? null
            : new LocalFile
            {
                Id = file.Id,
                FolderId = file.FolderId,
                Name = file.Name,
                Description = file.Description,
                Path = file.Path,
                MimeType = file.MimeType,
                CreatedBy = file.CreatedBy,
                Size = file.Size,
                CreatedOn = file.CreatedOn,
                DeletedOn = file.DeletedOn,
                Folder = CreateFolder(folder: file.Folder),
                Contents = file.Contents?.Select(selector: CreateLocalFileContent)
                                                                             .ToList() ?? [],
            };

    private static LocalFile CreateLocalFileWithFolderRoles(FileEntity file) =>
        file == null
            ? null
            : new LocalFile
            {
                Id = file.Id,
                FolderId = file.FolderId,
                Name = file.Name,
                Description = file.Description,
                Path = file.Path,
                MimeType = file.MimeType,
                CreatedBy = file.CreatedBy,
                Size = file.Size,
                CreatedOn = file.CreatedOn,
                DeletedOn = file.DeletedOn,
                Folder = CreateFolderWithRoles(folder: file.Folder),
                Contents = file.Contents?.Select(selector: CreateLocalFileContent)
                                                                             .ToList() ?? [],
            };

    private static Folder CreateFolder(Folder folder) =>
        folder == null
            ? null
            : new Folder
            {
                Id = folder.Id,
                AppId = folder.AppId,
                ParentId = folder.ParentId,
                Name = folder.Name,
                Path = folder.Path,
                DeletedOn = folder.DeletedOn,
            };

    private static Folder CreateFolderWithRoles(Folder folder) =>
        folder == null
            ? null
            : new Folder
            {
                Id = folder.Id,
                AppId = folder.AppId,
                ParentId = folder.ParentId,
                Name = folder.Name,
                Path = folder.Path,
                DeletedOn = folder.DeletedOn,
                Roles = folder.Roles?.Select(selector: folderRole => new FolderRole
                {
                    FolderId = folderRole.FolderId,
                    RoleId = folderRole.RoleId,
                    Role = folderRole.Role == null
                        ? null
                        : new Role
                        {
                            Id = folderRole.Role.Id,
                            AppId = folderRole.Role.AppId,
                            Name = folderRole.Role.Name,
                            Description = folderRole.Role.Description,
                            Privs = folderRole.Role.Privs,
                        },
                })
                  .ToList(),
            };

    private static FileContent CreateLocalFileContent(FileContent content) =>
        content == null
            ? null
            : new FileContent
            {
                Id = content.Id,
                FileId = content.FileId,
                Description = content.Description,
                Size = content.Size,
                CreatedBy = content.CreatedBy,
                CreatedOn = content.CreatedOn,
                Version = content.Version,
                RawData = content.RawData,
            };

    private static FileEntity CreateLocalFileEntity(LocalFile file, bool includeId)
    {
        if (file == null)
        {
            return null;
        }

        return new FileEntity
        {
            Id = includeId ? file.Id : Guid.Empty,
            FolderId = file.FolderId,
            Name = file.Name,
            Description = file.Description,
            Path = file.Path,
            MimeType = file.MimeType,
            CreatedBy = file.CreatedBy,
            Size = file.Size,
            CreatedOn = file.CreatedOn,
            DeletedOn = file.DeletedOn,
        };
    }

    private IQueryable<LocalFile> GetAllValue(bool ignoreFilters = false) =>
        GetAll(ignoreFilters: ignoreFilters);

    private ValueTask<LocalFile> UpdateForAppFileValueAsync(LocalFile file) =>
        UpdateForAppFileAsync(file: file);
}