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

internal class FolderService(IFolderBroker folderBroker, IAuthorizationBroker authorizationBroker)
    : IFolderService
{
    public Folder Get(Guid id)
    {
        Folder folder = GetAll()
            .FirstOrDefault(predicate: i => i.Id == id);

        if (folder is not null)
        {
            return folder;
        }

        Folder unrestrictedFolder = GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: i => i.Id == id);

        if (unrestrictedFolder is not null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;
    }

    public IQueryable<Folder> GetAll(bool ignoreFilters = false) =>
        folderBroker.GetAllFolders(ignoreFilters: ignoreFilters);

    public Folder GetWithRoles(Guid id, bool ignoreFilters = false) =>
        CreateFolder(folder: folderBroker.GetFolderWithRoles(id: id, ignoreFilters: ignoreFilters));

    public Folder GetForUpdate(Guid id, bool ignoreFilters = false) =>
        CreateFolderForUpdate(folder: folderBroker.GetFolderForUpdate(id: id, ignoreFilters: ignoreFilters));

    public Folder GetByPath(int appId, string path, bool ignoreFilters = false) =>
        CreateFolder(folder: folderBroker.GetFolderByPath(appId: appId, path: path, ignoreFilters: ignoreFilters));

    public Folder GetByPathWithRoles(int appId, string path, bool ignoreFilters = false) =>
        CreateFolder(folder: folderBroker.GetFolderByPathWithRoles(appId: appId, path: path, ignoreFilters: ignoreFilters));

    public Folder GetByPathWithParentAndRoles(int appId, string path, bool ignoreFilters = false) =>
        CreateFolderWithParent(folder: folderBroker.GetFolderByPathWithParentAndRoles(appId: appId, path: path, ignoreFilters: ignoreFilters));

    public Folder GetByPathWithRolesAndFilesAndContents(
        int appId,
        string path,
        bool ignoreFilters = false
    ) =>
        CreateFolderWithRolesAndFiles(
            folder: folderBroker.GetFolderByPathWithRolesAndFilesAndContents(appId: appId, path: path, ignoreFilters: ignoreFilters)
        );

    public Folder GetByPathWithSubFoldersAndFiles(
        int appId,
        string path,
        bool ignoreFilters = false
    ) =>
        CreateFolderForMove(folder: folderBroker.GetFolderByPathWithSubFoldersAndFiles(appId: appId, path: path, ignoreFilters: ignoreFilters));

    public async ValueTask<Folder> AddAsync(Folder folder)
    {
        authorizationBroker.Authorize(appId: folder.AppId, privilege: $"{nameof(Folder)}_create");
        Folder newFolder = CreateStorageFolderForAdd(folder: folder);
        Folder result = await folderBroker.AddFolderAsync(entity: newFolder);
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
        Folder newFolder = CreateStorageFolderForAdd(folder: folder);
        Folder result = await folderBroker.AddFolderAsync(entity: newFolder);
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
        authorizationBroker.Authorize(appId: folder.AppId, privilege: $"{nameof(Folder)}_update");
        return await UpdateForAppAsync(folder: folder);
    }

    public async ValueTask<Folder> UpdateForAppAsync(Folder folder)
    {
        Folder updateFolder = CreateStorageFolder(folder: folder, includeId: true);

        Folder result = await folderBroker.UpdateFolderAsync(entity: updateFolder);
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
        Folder folder = GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: foundFolder => foundFolder.Id == id);

        if (folder is null)
        {
            return;
        }

        authorizationBroker.Authorize(appId: folder.AppId, privilege: $"{nameof(Folder)}_delete");
        _ = await folderBroker.DeleteFolderAsync(entity: CreateStorageFolder(folder: folder, includeId: true));
    }

    public ValueTask DeleteAllForAppAsync(IEnumerable<Folder> folders) =>
        folderBroker.DeleteAllFoldersAsync(
            items: folders?.Select(selector: folder => CreateStorageFolder(folder: folder, includeId: true)) ?? []);

    public ValueTask DeleteAllByAppIdAsync(int appId) =>
        folderBroker.DeleteAllFoldersByAppIdAsync(appId: appId);

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
                App = CreateApp(app: folder.App),
                SubFolders = folder.SubFolders?.Select(selector: CreateFolder)
                                                                              .ToList(),
                Files = folder.Files?.Select(selector: CreateFile)
                                                                  .ToList(),
                Roles = folder.Roles?.Select(selector: CreateFolderRole)
                                                                        .ToList(),
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
                App = CreateApp(app: folder.App),
                Parent = CreateFolder(folder: folder.Parent),
                SubFolders = folder.SubFolders?.Select(selector: CreateFolder)
                                                                              .ToList(),
                Files = folder.Files?.Select(selector: CreateFile)
                                                                  .ToList(),
                Roles = folder.Roles?.Select(selector: CreateFolderRole)
                                                                        .ToList(),
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
                Parent = CreateFolder(folder: folder.Parent),
                Roles = folder.Roles?.Select(selector: CreateFolderRole)
                                                                        .ToList(),
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
                SubFolders = folder.SubFolders?.Select(selector: CreateFolder)
                                                                              .ToList(),
                Files = folder.Files?.Select(selector: CreateFile)
                                                                  .ToList(),
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
                App = CreateApp(app: folder.App),
                Files = folder.Files?.Select(selector: file => new LocalFile
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
                    Contents = file.Contents?.Select(selector: content => new FileContent
                    {
                        Id = content.Id,
                        FileId = content.FileId,
                        Description = content.Description,
                        Size = content.Size,
                        CreatedBy = content.CreatedBy,
                        CreatedOn = content.CreatedOn,
                        Version = content.Version,
                        RawData = content.RawData,
                    })
                      .ToList(),
                })
                  .ToList(),
                Roles = folder.Roles?.Select(selector: CreateFolderRole)
                                                                        .ToList(),
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
        Folder newFolder = CreateStorageFolder(folder: folder, includeId: false);

        if (newFolder == null)
        {
            return null;
        }

        newFolder.Roles = folder.Roles?.Select(selector: CreateFolderRole)
            .ToList();

        return newFolder;
    }

}