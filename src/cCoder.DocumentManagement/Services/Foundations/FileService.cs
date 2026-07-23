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

internal class FileService(IFileBroker fileBroker, IAuthorizationBroker authorizationBroker)
    : IFileService
{
    public LocalFile Get(Guid id)
    {
        LocalFile file = GetAll()
            .FirstOrDefault(predicate: i => i.Id == id);

        if (file is not null)
        {
            return file;
        }

        LocalFile unrestrictedFile = GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: i => i.Id == id);

        if (unrestrictedFile is not null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;
    }

    public IQueryable<LocalFile> GetAll(bool ignoreFilters = false) =>
        fileBroker.SelectAllFiles(ignoreFilters: ignoreFilters);

    public Guid[] GetIdsByFolderIds(Guid[] folderIds, bool ignoreFilters = false) =>
        fileBroker.GetFileIdsByFolderIds(folderIds: folderIds, ignoreFilters: ignoreFilters);

    public LocalFile GetWithFolderAndContents(Guid id, bool ignoreFilters = false) =>
        CreateFile(file: fileBroker.SelectFileWithFolderAndContents(id: id, ignoreFilters: ignoreFilters));

    public LocalFile GetWithFolderRolesAndContents(Guid id, bool ignoreFilters = false) =>
        CreateFileWithFolderRoles(file: fileBroker.SelectFileWithFolderRolesAndContents(id: id, ignoreFilters: ignoreFilters));

    public LocalFile GetByPath(int appId, string path, bool ignoreFilters = false) =>
        CreateFile(file: fileBroker.SelectFileByPath(appId: appId, path: path, ignoreFilters: ignoreFilters));

    public LocalFile GetByPathWithFolderAndContents(
        int appId,
        string path,
        bool ignoreFilters = false
    ) =>
        CreateFile(file: fileBroker.SelectFileByPathWithFolderAndContents(appId: appId, path: path, ignoreFilters: ignoreFilters));

    public LocalFile GetByPathWithFolderRolesAndContents(
        int appId,
        string path,
        bool ignoreFilters = false
    ) =>
        CreateFileWithFolderRoles(
            file: fileBroker.SelectFileByPathWithFolderRolesAndContents(appId: appId, path: path, ignoreFilters: ignoreFilters)
        );

    public IQueryable<LocalFile> Search(int appId, byte[] needle) =>
        fileBroker.SelectFilesByContent(appId: appId, needle: needle)
                                                            .AsEnumerable()
                                                                           .Select(selector: CreateFile)
                                                                                                        .AsQueryable();

    public async ValueTask<LocalFile> AddAsync(LocalFile file)
    {
        FileEntity newFileEntity = CreateStorageFile(file: file, includeId: false);

        authorizationBroker.Authorize(
            appId: fileBroker.GetAppId(entity: newFileEntity),
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
    }

    public async ValueTask<LocalFile> UpdateAsync(LocalFile file)
    {
        FileEntity updateFileEntity = CreateStorageFile(file: file, includeId: true);

        authorizationBroker.Authorize(
            appId: fileBroker.GetAppId(entity: updateFileEntity),
            privilege: $"{nameof(cCoder.Data.Models.DMS.File)}_update"
        );

        return await UpdateForAppAsync(file: file);
    }

    public async ValueTask<LocalFile> UpdateForAppAsync(LocalFile file)
    {
        FileEntity updateFileEntity = CreateStorageFile(file: file, includeId: true);
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
    }

    public async ValueTask DeleteAsync(Guid id)
    {
        LocalFile file = GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: foundFile => foundFile.Id == id);

        if (file is null)
        {
            return;
        }

        authorizationBroker.Authorize(
            appId: fileBroker.GetAppId(entity: new FileEntity
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
    }

    public ValueTask DeleteAllForAppAsync(IEnumerable<LocalFile> items) =>
        fileBroker.DeleteAllFilesAsync(
            items: items?.Select(selector: file => CreateStorageFile(file: file, includeId: true)) ?? []);

    private static LocalFile CreateFile(FileEntity file) =>
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
                Contents = file.Contents?.Select(selector: CreateFileContent)
                                                                             .ToList() ?? [],
            };

    private static LocalFile CreateFileWithFolderRoles(FileEntity file) =>
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
                Contents = file.Contents?.Select(selector: CreateFileContent)
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

    private static FileContent CreateFileContent(FileContent content) =>
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

    private static FileEntity CreateStorageFile(LocalFile file, bool includeId)
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

}