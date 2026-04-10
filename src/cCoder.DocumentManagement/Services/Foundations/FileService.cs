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
        LocalFile file = GetAll().FirstOrDefault(i => i.Id == id);
        if (file is not null)
            return file;

        LocalFile unrestrictedFile = GetAll(true).FirstOrDefault(i => i.Id == id);
        if (unrestrictedFile is not null)
            throw new SecurityException("Access Denied!");

        return null;
    }

    public IQueryable<LocalFile> GetAll(bool ignoreFilters = false) =>
        fileBroker.GetAllFiles(ignoreFilters);

    public Guid[] GetIdsByFolderIds(Guid[] folderIds, bool ignoreFilters = false) =>
        fileBroker.GetFileIdsByFolderIds(folderIds, ignoreFilters);

    public LocalFile GetWithFolderAndContents(Guid id, bool ignoreFilters = false) =>
        CreateFile(fileBroker.GetFileWithFolderAndContents(id, ignoreFilters));

    public LocalFile GetWithFolderRolesAndContents(Guid id, bool ignoreFilters = false) =>
        CreateFileWithFolderRoles(fileBroker.GetFileWithFolderRolesAndContents(id, ignoreFilters));

    public LocalFile GetByPath(int appId, string path, bool ignoreFilters = false) =>
        CreateFile(fileBroker.GetFileByPath(appId, path, ignoreFilters));

    public LocalFile GetByPathWithFolderAndContents(
        int appId,
        string path,
        bool ignoreFilters = false
    ) => CreateFile(fileBroker.GetFileByPathWithFolderAndContents(appId, path, ignoreFilters));

    public LocalFile GetByPathWithFolderRolesAndContents(
        int appId,
        string path,
        bool ignoreFilters = false
    ) =>
        CreateFileWithFolderRoles(
            fileBroker.GetFileByPathWithFolderRolesAndContents(appId, path, ignoreFilters)
        );

    public IQueryable<LocalFile> Search(int appId, byte[] needle) =>
        fileBroker.SearchFiles(appId, needle).AsEnumerable().Select(CreateFile).AsQueryable();

    public async ValueTask<LocalFile> AddAsync(LocalFile file)
    {
        FileEntity newFileEntity = CreateStorageFile(file, includeId: false);
        authorizationBroker.Authorize(
            fileBroker.GetAppId(newFileEntity),
            $"{nameof(cCoder.Data.Models.DMS.File)}_create"
        );
        string currentUserId = authorizationBroker.GetCurrentUser().Id;
        DateTimeOffset now = DateTimeOffset.UtcNow;
        newFileEntity.CreatedOn = now;
        newFileEntity.CreatedBy = currentUserId;

        newFileEntity = await fileBroker.AddFileAsync(newFileEntity);
        CopyFileChildren(newFileEntity, file);
        return newFileEntity;
    }

    public async ValueTask<LocalFile> UpdateAsync(LocalFile file)
    {
        FileEntity updateFileEntity = CreateStorageFile(file, includeId: true);
        authorizationBroker.Authorize(
            fileBroker.GetAppId(updateFileEntity),
            $"{nameof(cCoder.Data.Models.DMS.File)}_update"
        );

        updateFileEntity = await fileBroker.UpdateFileAsync(updateFileEntity);
        CopyFileChildren(updateFileEntity, file);
        return updateFileEntity;
    }

    public async ValueTask DeleteAsync(Guid id)
    {
        LocalFile file = GetAll(ignoreFilters: true).FirstOrDefault(foundFile => foundFile.Id == id);

        if (file is null)
        {
            return;
        }

        authorizationBroker.Authorize(
            fileBroker.GetAppId(new FileEntity
            {
                Id = file.Id,
                FolderId = file.FolderId,
            }),
            $"file_delete"
        );
        _ = await fileBroker.DeleteFileAsync(new FileEntity
        {
            Id = file.Id,
            FolderId = file.FolderId,
        });
    }

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
                Folder = CreateFolder(file.Folder),
                Contents = file.Contents?.Select(CreateFileContent).ToList() ?? [],
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
                Folder = CreateFolderWithRoles(file.Folder),
                Contents = file.Contents?.Select(CreateFileContent).ToList() ?? [],
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
                Roles = folder.Roles?.Select(folderRole => new FolderRole
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
                }).ToList(),
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

    private static void CopyFileChildren(FileEntity target, LocalFile source)
    {
        if (target == null || source == null)
        {
            return;
        }

        target.Folder = source.Folder;
        target.Contents = source.Contents;
    }
}













