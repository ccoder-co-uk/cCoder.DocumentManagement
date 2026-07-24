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

internal partial class FolderService(IFolderBroker folderBroker, IAuthorizationBroker authorizationBroker)
    : IFolderService
{
    public Folder Get(Guid folderId)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [folderId]);
            Folder folder = GetAll()
    .FirstOrDefault(predicate: i => i.Id == folderId);


            if (folder is not null)
            {
                return folder;
            }


            Folder unrestrictedFolder = GetAll(ignoreFilters: true)
                .FirstOrDefault(predicate: i => i.Id == folderId);


            if (unrestrictedFolder is not null)
            {
                throw new SecurityException(message: "Access Denied!");
            }


            return null;

        });

    public IQueryable<Folder> GetAll(bool ignoreFilters = false)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [ignoreFilters]);
            return folderBroker.SelectAllFolders(ignoreFilters: ignoreFilters);
        });

    public Folder GetWithRoles(Guid folderId, bool ignoreFilters = false)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [folderId, ignoreFilters]);
            return CreateFolder(folder: folderBroker.SelectFolderWithRoles(folderId: folderId, ignoreFilters: ignoreFilters));
        });

    public Folder GetForUpdate(Guid folderId, bool ignoreFilters = false)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [folderId, ignoreFilters]);
            return CreateFolderForUpdate(folder: folderBroker.SelectFolderForUpdate(folderId: folderId, ignoreFilters: ignoreFilters));
        });

    public Folder GetByPath(int appId, string path, bool ignoreFilters = false)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [appId, path, ignoreFilters]);
            return CreateFolder(folder: folderBroker.SelectFolderByPath(appId: appId, path: path, ignoreFilters: ignoreFilters));
        });

    public Folder GetByPathWithRoles(int appId, string path, bool ignoreFilters = false)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [appId, path, ignoreFilters]);
            return CreateFolder(folder: folderBroker.SelectFolderByPathWithRoles(appId: appId, path: path, ignoreFilters: ignoreFilters));
        });

    public Folder GetByPathWithParentAndRoles(int appId, string path, bool ignoreFilters = false)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [appId, path, ignoreFilters]);
            return CreateFolderWithParent(folder: folderBroker.SelectFolderByPathWithParentAndRoles(appId: appId, path: path, ignoreFilters: ignoreFilters));
        });

    public Folder GetByPathWithRolesAndFilesAndContents(
        int appId,
        string path,
        bool ignoreFilters = false
    )
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [appId, path, ignoreFilters]);
            return CreateFolderWithRolesAndFiles(
                folder: folderBroker.SelectFolderByPathWithRolesAndFilesAndContents(appId: appId, path: path, ignoreFilters: ignoreFilters)
            );
        });

    public Folder GetByPathWithSubFoldersAndFiles(
        int appId,
        string path,
        bool ignoreFilters = false
    )
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [appId, path, ignoreFilters]);
            return CreateFolderForMove(folder: folderBroker.SelectFolderByPathWithSubFoldersAndFiles(appId: appId, path: path, ignoreFilters: ignoreFilters));
        });

    public ValueTask<Folder> AddFolderAsync(Folder newFolder)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [newFolder]);
            authorizationBroker.Authorize(appId: newFolder.AppId, privilege: $"{nameof(Folder)}_create");

            Folder storageFolder = CreateStorageFolderForAdd(folder: newFolder);

            Folder result = await folderBroker.InsertFolderAsync(newFolder: storageFolder);

            newFolder.Id = result.Id;

            newFolder.AppId = result.AppId;

            newFolder.ParentId = result.ParentId;

            newFolder.Name = result.Name;

            newFolder.Path = result.Path;

            newFolder.DeletedOn = result.DeletedOn;

            return newFolder;

        });

    public ValueTask<Folder> AddForPathBuildFolderAsync(Folder newFolder)
=>
        TryCatch(operation: (Func<ValueTask<Folder>>)(async () =>
        {
            ValidateInputs(inputs: (object[])[newFolder]);
            Folder storageFolder = CreateStorageFolderForAdd(folder: newFolder);

            Folder result = await folderBroker.InsertFolderAsync(newFolder: storageFolder);

            newFolder.Id = result.Id;

            newFolder.AppId = result.AppId;

            newFolder.ParentId = result.ParentId;

            newFolder.Name = result.Name;

            newFolder.Path = result.Path;

            newFolder.DeletedOn = result.DeletedOn;

            return newFolder;

        }));

    public ValueTask<Folder> UpdateFolderAsync(Folder updatedFolder)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [updatedFolder]);
            authorizationBroker.Authorize(appId: updatedFolder.AppId, privilege: $"{nameof(Folder)}_update");

            return await UpdateForAppFolderAsync(updatedFolder: updatedFolder);

        });

    public ValueTask<Folder> UpdateForAppFolderAsync(Folder updatedFolder)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [updatedFolder]);
            Folder updateFolder = CreateStorageFolder(folder: updatedFolder, includeId: true);


            Folder result = await folderBroker.UpdateFolderAsync(updatedFolder: updateFolder);

            updatedFolder.Id = result.Id;

            updatedFolder.AppId = result.AppId;

            updatedFolder.ParentId = result.ParentId;

            updatedFolder.Name = result.Name;

            updatedFolder.Path = result.Path;

            updatedFolder.DeletedOn = result.DeletedOn;

            return updatedFolder;

        });

    public ValueTask DeleteAsync(Guid folderId)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [folderId]);
            Folder folder = GetAll(ignoreFilters: true)
    .FirstOrDefault(predicate: foundFolder => foundFolder.Id == folderId);


            if (folder is null)
            {
                return;
            }


            authorizationBroker.Authorize(appId: folder.AppId, privilege: $"{nameof(Folder)}_delete");

            _ = await folderBroker.DeleteFolderAsync(deletedFolder: CreateStorageFolder(folder: folder, includeId: true));

        });

    public ValueTask DeleteAllForAppFolderAsync(IEnumerable<Folder> deletedFolder)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [deletedFolder]);
            return folderBroker.DeleteAllFoldersAsync(
                deletedFolder: deletedFolder?.Select(selector: folder => CreateStorageFolder(folder: folder, includeId: true)) ?? []);
        });

    public ValueTask DeleteAllByAppIdAsync(int appId)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [appId]);
            return folderBroker.DeleteAllFoldersByAppIdAsync(appId: appId);
        });

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
                Files = folder.Files?.Select(selector: CreateLocalFile)
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
                Files = folder.Files?.Select(selector: CreateLocalFile)
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
                Files = folder.Files?.Select(selector: CreateLocalFile)
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

    private static LocalFile CreateLocalFile(LocalFile file) =>
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
