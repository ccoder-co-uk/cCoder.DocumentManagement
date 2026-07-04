using System.Security;
using cCoder.DocumentManagement.Brokers.Storage;
using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using IAuthorizationBroker = cCoder.DocumentManagement.Brokers.IAuthorizationBroker;
using LocalFile = cCoder.Data.Models.DMS.File;


namespace cCoder.DocumentManagement.Services.Foundations;

internal class FolderService(IFolderBroker folderBroker, IAuthorizationBroker authorizationBroker)
    : IFolderService
{
    public Folder Get(Guid id)
    {
        Folder folder = GetAll().FirstOrDefault(i => i.Id == id);
        if (folder is not null)
            return folder;

        Folder unrestrictedFolder = GetAll(true).FirstOrDefault(i => i.Id == id);
        if (unrestrictedFolder is not null)
            throw new SecurityException("Access Denied!");

        return null;
    }

    public IQueryable<Folder> GetAll(bool ignoreFilters = false) =>
        folderBroker.GetAllFolders(ignoreFilters);

    public Folder GetWithRoles(Guid id, bool ignoreFilters = false) =>
        CreateFolder(folderBroker.GetFolderWithRoles(id, ignoreFilters));

    public Folder GetForUpdate(Guid id, bool ignoreFilters = false) =>
        CreateFolderForUpdate(folderBroker.GetFolderForUpdate(id, ignoreFilters));

    public Folder GetByPath(int appId, string path, bool ignoreFilters = false) =>
        CreateFolder(folderBroker.GetFolderByPath(appId, path, ignoreFilters));

    public Folder GetByPathWithRoles(int appId, string path, bool ignoreFilters = false) =>
        CreateFolder(folderBroker.GetFolderByPathWithRoles(appId, path, ignoreFilters));

    public Folder GetByPathWithParentAndRoles(int appId, string path, bool ignoreFilters = false) =>
        CreateFolderWithParent(folderBroker.GetFolderByPathWithParentAndRoles(appId, path, ignoreFilters));

    public Folder GetByPathWithRolesAndFilesAndContents(
        int appId,
        string path,
        bool ignoreFilters = false
    ) =>
        CreateFolderWithRolesAndFiles(
            folderBroker.GetFolderByPathWithRolesAndFilesAndContents(appId, path, ignoreFilters)
        );

    public Folder GetByPathWithSubFoldersAndFiles(
        int appId,
        string path,
        bool ignoreFilters = false
    ) => CreateFolderForMove(folderBroker.GetFolderByPathWithSubFoldersAndFiles(appId, path, ignoreFilters));

    public async ValueTask<Folder> AddAsync(Folder folder)
    {
        authorizationBroker.Authorize(folder.AppId, $"{nameof(Folder)}_create");
        Folder newFolder = CreateStorageFolderForAdd(folder);
        Folder result = await folderBroker.AddFolderAsync(newFolder);
        folder.Id = result.Id;
        folder.AppId = result.AppId;
        folder.ParentId = result.ParentId;
        folder.Name = result.Name;
        folder.Path = result.Path;
        folder.DeletedOn = result.DeletedOn;
        return folder;
    }

    public async ValueTask<Folder> AddForPathBuildAsync(Folder folder)
    {
        Folder newFolder = CreateStorageFolderForAdd(folder);
        Folder result = await folderBroker.AddFolderAsync(newFolder);
        folder.Id = result.Id;
        folder.AppId = result.AppId;
        folder.ParentId = result.ParentId;
        folder.Name = result.Name;
        folder.Path = result.Path;
        folder.DeletedOn = result.DeletedOn;
        return folder;
    }

    public async ValueTask<Folder> UpdateAsync(Folder folder)
    {
        authorizationBroker.Authorize(folder.AppId, $"{nameof(Folder)}_update");
        return await UpdateForAppAsync(folder);
    }

    public async ValueTask<Folder> UpdateForAppAsync(Folder folder)
    {
        Folder updateFolder = CreateStorageFolder(folder, includeId: true);

        Folder result = await folderBroker.UpdateFolderAsync(updateFolder);
        folder.Id = result.Id;
        folder.AppId = result.AppId;
        folder.ParentId = result.ParentId;
        folder.Name = result.Name;
        folder.Path = result.Path;
        folder.DeletedOn = result.DeletedOn;
        return folder;
    }

    public async ValueTask DeleteAsync(Guid id)
    {
        Folder folder = GetAll(ignoreFilters: true).FirstOrDefault(foundFolder => foundFolder.Id == id);

        if (folder is null)
        {
            return;
        }

        authorizationBroker.Authorize(folder.AppId, $"{nameof(Folder)}_delete");
        _ = await folderBroker.DeleteFolderAsync(CreateStorageFolder(folder, includeId: true));
    }

    public ValueTask DeleteAllForAppAsync(IEnumerable<Folder> folders) =>
        folderBroker.DeleteAllFoldersAsync(
            folders?.Select(folder => CreateStorageFolder(folder, includeId: true)) ?? []);

    public ValueTask DeleteAllByAppIdAsync(int appId) =>
        folderBroker.DeleteAllFoldersByAppIdAsync(appId);

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
                App = CreateApp(folder.App),
                SubFolders = folder.SubFolders?.Select(CreateFolder).ToList(),
                Files = folder.Files?.Select(CreateFile).ToList(),
                Roles = folder.Roles?.Select(CreateFolderRole).ToList(),
            };

    private static Folder CreateFolderForUpdate(Folder folder) =>
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
                App = CreateApp(folder.App),
                Parent = CreateFolder(folder.Parent),
                SubFolders = folder.SubFolders?.Select(CreateFolder).ToList(),
                Files = folder.Files?.Select(CreateFile).ToList(),
                Roles = folder.Roles?.Select(CreateFolderRole).ToList(),
            };

    private static Folder CreateFolderWithParent(Folder folder) =>
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
                Parent = CreateFolder(folder.Parent),
                Roles = folder.Roles?.Select(CreateFolderRole).ToList(),
            };

    private static Folder CreateFolderForMove(Folder folder) =>
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
                SubFolders = folder.SubFolders?.Select(CreateFolder).ToList(),
                Files = folder.Files?.Select(CreateFile).ToList(),
            };

    private static Folder CreateFolderWithRolesAndFiles(Folder folder) =>
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
                App = CreateApp(folder.App),
                Files = folder.Files?.Select(file => new LocalFile
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
                    Contents = file.Contents?.Select(content => new FileContent
                    {
                        Id = content.Id,
                        FileId = content.FileId,
                        Description = content.Description,
                        Size = content.Size,
                        CreatedBy = content.CreatedBy,
                        CreatedOn = content.CreatedOn,
                        Version = content.Version,
                        RawData = content.RawData,
                    }).ToList(),
                }).ToList(),
                Roles = folder.Roles?.Select(CreateFolderRole).ToList(),
            };

    private static App CreateApp(App app) =>
        app == null ? null : new App { Id = app.Id, Name = app.Name };

    private static LocalFile CreateFile(LocalFile file) =>
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
            };

    private static FolderRole CreateFolderRole(FolderRole folderRole) =>
        folderRole == null
            ? null
            : new FolderRole
            {
                FolderId = folderRole.FolderId,
                RoleId = folderRole.RoleId,
                Role = folderRole.Role == null ? null : new Role
                {
                    Id = folderRole.Role.Id,
                    AppId = folderRole.Role.AppId,
                    Name = folderRole.Role.Name,
                    Description = folderRole.Role.Description,
                    Privs = folderRole.Role.Privs,
                },
            };

    private static Folder CreateStorageFolder(Folder folder, bool includeId)
    {
        if (folder == null)
        {
            return null;
        }

        return new Folder
        {
            Id = includeId ? folder.Id : Guid.Empty,
            AppId = folder.AppId,
            ParentId = folder.ParentId,
            Name = folder.Name,
            Path = folder.Path,
            DeletedOn = folder.DeletedOn
        };
    }

    private static Folder CreateStorageFolderForAdd(Folder folder)
    {
        Folder newFolder = CreateStorageFolder(folder, includeId: false);
        if (newFolder == null)
        {
            return null;
        }

        newFolder.Roles = folder.Roles?.Select(CreateFolderRole).ToList();
        return newFolder;
    }

}












