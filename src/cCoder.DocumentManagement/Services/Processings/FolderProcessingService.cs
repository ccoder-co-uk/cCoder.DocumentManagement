// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.IO.Compression;
using System.Security;
using cCoder.DocumentManagement.Brokers;
using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Foundations;

namespace cCoder.DocumentManagement.Services.Processings;

internal partial class FolderProcessingService(IFolderService service, IFolderRoleService folderRoleService, IRoleService roleService, IFileService fileService, IFileContentService fileContentService, IFileProcessingService fileProcessingService, IAuthorizationBroker authorizationBroker) : IFolderProcessingService
{
    private sealed record FolderArchiveData(ILookup<Guid?, Folder> SubFoldersByParentId, ILookup<Guid, cCoder.Data.Models.DMS.File> FilesByFolderId, ILookup<Guid, FileContent> FileContentsByFileId);

    private User GetCurrentUser() =>
        authorizationBroker.GetCurrentUser();

    public Folder Get(Guid folderId)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [folderId]);
            return service.Get(folderId: folderId);

        });

    public IQueryable<Folder> GetAll(bool ignoreFilters = false)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [ignoreFilters]);
            return service.GetAll(ignoreFilters: ignoreFilters);

        });

    public ValueTask<List<Result<Guid?>>> CopyAsync(string source, string destination, int sourceAppId, int destAppId)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [source, destination, sourceAppId, destAppId]);
            Folder sourceFolder = service.GetByPathWithRolesAndFilesAndContents(appId: sourceAppId, path: source.ToLower(), ignoreFilters: true);

            Folder destinationFolder = service.GetByPathWithRolesAndFilesAndContents(appId: destAppId, path: destination.ToLower(), ignoreFilters: true);


            if (sourceFolder == null)
            {
                throw new InvalidOperationException(message: "Source folder doesn't exist.");
            }


            if ((!sourceFolder.UserCan(user: GetCurrentUser(), privilege: "file_update") || !sourceFolder.UserCan(user: GetCurrentUser(), privilege: "file_create")) && !GetCurrentUser().IsAdminOfApp(appId: destAppId))
            {
                throw new SecurityException(message: "Access Denied!");
            }


            if (destinationFolder == null)
            {
                throw new InvalidOperationException(message: "Destination folder doesn't exist.");
            }


            if ((!destinationFolder.UserCan(user: GetCurrentUser(), privilege: "file_update") || !destinationFolder.UserCan(user: GetCurrentUser(), privilege: "file_create")) && !GetCurrentUser().IsAdminOfApp(appId: destAppId))
            {
                throw new SecurityException(message: "Access Denied!");
            }


            App destinationApp = new App
            {
                Id = destAppId
            };


            cCoder.Data.Models.DMS.File[] sourceFiles = sourceFolder.Files?.ToArray() ?? Array.Empty<cCoder.Data.Models.DMS.File>();

            List<Result<Guid?>> results = new List<Result<Guid?>>();

            cCoder.Data.Models.DMS.File[] array = sourceFiles;


            foreach (cCoder.Data.Models.DMS.File entry in array)
            {
                using MemoryStream sourceStream = new MemoryStream(buffer: entry.Contents.OrderBy(keySelector: (FileContent k) => k.Version)
                    .FirstOrDefault().RawData);

                try
                {
                    await fileProcessingService.SaveAppPathAsync(app: destinationApp, path: new cCoder.DocumentManagement.Models.Path(path: destinationFolder.Path + "/" + entry.Name), content: sourceStream);

                    results.Add(item: new Result<Guid?>
                    {
                        Item = entry.Id,
                        Success = true,
                        Id = entry.Id.ToString()
                    });
                }
                catch (Exception ex)
                {
                    Exception ex2 = ex;

                    results.Add(item: new Result<Guid?>
                    {
                        Item = null,
                        Success = false,
                        Id = entry.Id.ToString(),
                        Message = ex2.Message
                    });
                }
            }


            return results;

        });

    public ValueTask<Folder> AddFolderAsync(Folder newFolder)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [newFolder]);
            if (newFolder.ParentId.HasValue)
            {
                Folder parent = GetValue(folderId: newFolder.ParentId.Value);

                if (parent == null)
                {
                    throw new SecurityException(message: "Access Denied!");
                }

                newFolder.Path = parent.Path + "/" + newFolder.Name;
            }
            else
            {
                newFolder.Path = newFolder.Name;
            }


            Folder existingFolder = GetAllValue(ignoreFilters: true)
                .FirstOrDefault(predicate: (Folder folder) => folder.AppId == newFolder.AppId && folder.Path.ToLower() == newFolder.Path.ToLower());


            if (existingFolder != null)
            {
                return existingFolder;
            }


            App app = new App
            {
                Id = newFolder.AppId
            };


            await SaveAppPathValueAsync(app: app, path: new cCoder.DocumentManagement.Models.Path(path: newFolder.Path));


            return GetAllValue(ignoreFilters: true)
                .FirstOrDefault(predicate: (Folder folder) => folder.AppId == newFolder.AppId && folder.Path.ToLower() == newFolder.Path.ToLower());

        });

    private async ValueTask<Folder> AddForAppFolderAsync(Folder newFolder)
    {
        if (newFolder.ParentId.HasValue)
        {
            Folder parent = service.GetWithRoles(folderId: newFolder.ParentId.Value, ignoreFilters: true);

            if (parent == null)
            {
                throw new InvalidOperationException(message: "Parent folder doesn't exist.");
            }

            newFolder.Path = parent.Path + "/" + newFolder.Name;
        }
        else if (string.IsNullOrWhiteSpace(value: newFolder.Path))
        {
            newFolder.Path = newFolder.Name;
        }

        Folder existingFolder = GetAllValue(ignoreFilters: true)
            .FirstOrDefault(predicate: folder =>
                folder.AppId == newFolder.AppId
                && folder.Path.ToLower() == newFolder.Path.ToLower());

        return existingFolder ?? await service.AddForPathBuildFolderAsync(newFolder: newFolder);
    }

    public ValueTask DeleteAsync(Guid folderId)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [folderId]);
            Folder folder = service.GetWithRoles(folderId: folderId, ignoreFilters: true);


            if (folder == null || (!GetCurrentUser().IsAdminOfApp(appId: folder.AppId) && !folder.UserCan(user: GetCurrentUser(), privilege: "folder_delete")))
            {
                throw new SecurityException(message: "Access Denied!");
            }


            await service.DeleteAsync(folderId: folderId);

        });

    public ValueTask<Folder> UpdateFolderAsync(Folder updatedFolder)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [updatedFolder]);
            Folder dbVersion = service.GetForUpdate(folderId: updatedFolder.Id, ignoreFilters: true);


            if (dbVersion != null && (GetCurrentUser().IsAdminOfApp(appId: dbVersion.AppId) || dbVersion.UserCan(user: GetCurrentUser(), privilege: "folder_update")))
            {
                return await UpdateInternalFolderAsync(updatedFolder: dbVersion, folder: updatedFolder, authorize: true);
            }


            throw new SecurityException(message: "Access Denied!");

        });

    private async ValueTask<Folder> UpdateForAppFolderAsync(Folder updatedFolder)
    {
        Folder dbVersion = service.GetForUpdate(folderId: updatedFolder.Id, ignoreFilters: true)
            ?? throw new InvalidOperationException(message: "Folder doesn't exist.");

        return await UpdateInternalFolderAsync(updatedFolder: dbVersion, folder: updatedFolder, authorize: false);
    }

    public ValueTask HandleFolderDeleteEventAsync(Folder folder)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [folder]);
            Folder dbFolder = GetAllValue(ignoreFilters: true)
    .FirstOrDefault(predicate: (Folder foundFolder) => foundFolder.Id == folder.Id);


            if (dbFolder != null)
            {
                string folderPathPrefix = dbFolder.Path + "/";

                Guid[] folderIds = (from foundFolder in GetAllValue(ignoreFilters: true)
                                    where foundFolder.Path == dbFolder.Path || foundFolder.Path.StartsWith(value: folderPathPrefix)
                                    select foundFolder.Id).ToArray();

                Guid[] fileIds = fileService.GetIdsByFolderIds(folderIds: folderIds, ignoreFilters: true);

                if (fileIds.Length != 0)
                {
                    await fileContentService.DeleteAllForFilesAsync(fileIds: fileIds);
                }
            }

        });

    public DMSResult GetFilesZippedAppPath(App app, IEnumerable<cCoder.DocumentManagement.Models.Path> paths)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [app, paths]);
            using MemoryStream memoryStream = new MemoryStream();


            using (ZipArchive zip = new ZipArchive(stream: memoryStream, mode: ZipArchiveMode.Create))
            {
                foreach (cCoder.DocumentManagement.Models.Path path in paths)
                {
                    if (path.IsToFile)
                    {
                        cCoder.Data.Models.DMS.File byPathWithFolderAndContents = fileService.GetByPathWithFolderAndContents(appId: app.Id, path: path.Lowered);

                        if (byPathWithFolderAndContents == null)
                        {
                            throw new SecurityException(message: "Access Denied!");
                        }

                        AddFileToZipFileContent(zip: zip, newFile: byPathWithFolderAndContents, fileContents: byPathWithFolderAndContents.Contents);
                        continue;
                    }

                    Folder byPath = service.GetByPath(appId: app.Id, path: path.Lowered);

                    if (byPath == null)
                    {
                        throw new SecurityException(message: "Access Denied!");
                    }

                    FolderArchiveData folderArchiveData = LoadFolderArchiveData(appId: app.Id, rootPath: byPath.Path, ignoreFilters: false);
                    AddFolderToZipFileFileContent(zip: zip, newFolder: byPath, subFoldersByParentId: folderArchiveData.SubFoldersByParentId, filesByFolderId: folderArchiveData.FilesByFolderId, fileContentsByFileId: folderArchiveData.FileContentsByFileId);
                }
            }


            return new DMSResult
            {
                MimeType = "application/zip",
                Data = new MemoryStream(buffer: memoryStream.ToArray())
            };

        });

    public DMSResult GetAppPath(App app, cCoder.DocumentManagement.Models.Path path, string search = "")
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [app, path, search]);
            if (path.IsToFile)
            {
                throw new InvalidOperationException(message: "To get a file, use file processing operations.");
            }


            Folder byPath = service.GetByPath(appId: app.Id, path: path.Lowered);


            if (byPath == null)
            {
                throw new SecurityException(message: "Access Denied!");
            }


            FolderArchiveData folderArchiveData = LoadFolderArchiveData(appId: app.Id, rootPath: byPath.Path, ignoreFilters: false);

            using MemoryStream memoryStream = new MemoryStream();


            using (ZipArchive zip = new ZipArchive(stream: memoryStream, mode: ZipArchiveMode.Create))
            {
                AddFolderToZipFileFileContent(zip: zip, newFolder: byPath, subFoldersByParentId: folderArchiveData.SubFoldersByParentId, filesByFolderId: folderArchiveData.FilesByFolderId, fileContentsByFileId: folderArchiveData.FileContentsByFileId, prefix: null, search: search);
            }


            return new DMSResult
            {
                MimeType = "application/zip",
                Data = new MemoryStream(buffer: memoryStream.ToArray())
            };

        });

    public ValueTask UnpackAppPathAsync(App app, cCoder.DocumentManagement.Models.Path path, Stream content, bool ignoreArchiveRoot = false)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [app, path, content, ignoreArchiveRoot]);
            Folder folder = await BuildPathAppAsync(app: app, folderPath: path);


            if (!GetCurrentUser().IsAdminOfApp(appId: app.Id) && !folder.UserCan(user: GetCurrentUser(), privilege: "file_create"))
            {
                throw new SecurityException(message: "Access Denied!");
            }


            using ZipArchive archive = new ZipArchive(stream: content, mode: ZipArchiveMode.Read);


            ZipArchiveEntry rootEntry = archive.Entries.OrderBy(keySelector: (ZipArchiveEntry zipArchiveEntry) => zipArchiveEntry.FullName.Split(separator: '/').Length)
                .First();


            string ignoreSegment = rootEntry.FullName;


            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                using Stream entryStream = entry.Open();
                string destinationPath = (ignoreArchiveRoot ? (path.FullPath + "/" + entry.FullName).Replace(oldValue: ignoreSegment, newValue: "") : (path.FullPath + "/" + entry.FullName));

                if (path.Lowered != destinationPath.ToLower())
                {
                    await fileProcessingService.SaveAppPathAsync(app: app, path: new cCoder.DocumentManagement.Models.Path(path: destinationPath), content: entryStream);
                }
            }

        });

    public ValueTask<IEnumerable<Result<Folder>>> AddOrUpdateFolder(IEnumerable<Folder> items)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [items]);
            List<Result<Folder>> results = new List<Result<Folder>>();


            foreach (Folder item in items)
            {
                try
                {
                    Folder savedItem = item.Id == Guid.Empty ? await AddFolderValueAsync(newFolder: item) : await UpdateFolderValueAsync(updatedFolder: item);

                    results.Add(item: new Result<Folder>
                    {
                        Success = true,
                        Item = savedItem,
                        Message = item.Id == Guid.Empty ? "Added Successfully" : "Updated Successfully"
                    });
                }
                catch (Exception ex)
                {
                    results.Add(item: new Result<Folder>
                    {
                        Success = false,
                        Item = item,
                        Message = ex.Message
                    });
                }
            }


            return (IEnumerable<Result<Folder>>)results;

        });

    public ValueTask<IEnumerable<Result<Folder>>> AddOrUpdateForAppFolderAsync(IEnumerable<Folder> items)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [items]);
            List<Result<Folder>> results = new List<Result<Folder>>();


            foreach (Folder item in items)
            {
                try
                {
                    Folder savedItem = item.Id == Guid.Empty
                        ? await AddForAppFolderAsync(newFolder: item)
                        : await UpdateForAppFolderAsync(updatedFolder: item);

                    results.Add(item: new Result<Folder>
                    {
                        Success = true,
                        Item = savedItem,
                        Message = item.Id == Guid.Empty ? "Added Successfully" : "Updated Successfully"
                    });
                }
                catch (Exception ex)
                {
                    results.Add(item: new Result<Folder>
                    {
                        Success = false,
                        Item = item,
                        Message = ex.Message
                    });
                }
            }


            return (IEnumerable<Result<Folder>>)results;

        });

    public ValueTask DeleteAllFolderAsync(IEnumerable<Folder> deletedFolder)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [deletedFolder]);
            foreach (Folder item in deletedFolder)
            {
                await DeleteValueAsync(folderId: item.Id);
            }

        });

    public ValueTask DeleteByAppIdAsync(int appId)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [appId]);
            return service.DeleteAllByAppIdAsync(appId: appId);
        });

    public ValueTask SaveAppPathAsync(App app, cCoder.DocumentManagement.Models.Path path)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [app, path]);
            await BuildPathAppAsync(app: app, folderPath: path);

        });

    public ValueTask DropAppPathAsync(App app, cCoder.DocumentManagement.Models.Path path)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [app, path]);
            await DropFolderAppPathAsync(app: app, path: path);

        });

    public ValueTask CopyAppPathAsync(App app, cCoder.DocumentManagement.Models.Path oldPath, cCoder.DocumentManagement.Models.Path newPath)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [app, oldPath, newPath]);
            if (oldPath.IsToFile)
            {
                throw new InvalidOperationException(message: "To copy a file, use file processing operations.");
            }


            await CopyFolderAppPathAsync(app: app, oldPath: oldPath, newPath: newPath);

        });

    public ValueTask MoveAppPathAsync(App app, cCoder.DocumentManagement.Models.Path oldPath, cCoder.DocumentManagement.Models.Path newPath)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [app, oldPath, newPath]);
            if (oldPath.IsToFile)
            {
                throw new InvalidOperationException(message: "To move a file, use file processing operations.");
            }


            Folder newParent = ((!string.IsNullOrEmpty(value: newPath.ParentPath.Lowered)) ? service.GetByPath(appId: app.Id, path: newPath.ParentPath.Lowered) : null);

            cCoder.DocumentManagement.Models.Path resolvedNewPath = new cCoder.DocumentManagement.Models.Path(path: (newParent != null) ? (newParent.Path + "/" + newPath.Name) : newPath.Name);

            await MoveFolderAppPathAsync(app: app, oldPath: oldPath, newPath: resolvedNewPath);

        });

    private async ValueTask<Folder> UpdateInternalFolderAsync(Folder updatedFolder, Folder folder, bool authorize)
    {
        string parentPath = new cCoder.DocumentManagement.Models.Path(path: folder.Path).ParentPath.FullPath;
        string newPath = ((!string.IsNullOrEmpty(value: parentPath)) ? "/" : "") + folder.Name.ToLower();

        Folder existingDestionFolder = GetAll()
            .FirstOrDefault(predicate: (Folder foundFolder) => foundFolder.Path == newPath && foundFolder.Path != updatedFolder.Path && foundFolder.AppId == folder.AppId);

        if (folder.ParentId != updatedFolder.ParentId)
        {
            updatedFolder.Parent = (folder.ParentId.HasValue ? service.Get(folderId: folder.ParentId.Value) : null);
        }

        updatedFolder.AppId = folder.AppId;
        updatedFolder.ParentId = folder.ParentId;
        updatedFolder.Name = folder.Name;
        updatedFolder.Path = folder.Path;
        updatedFolder.DeletedOn = folder.DeletedOn;
        updatedFolder.RecomputePaths();

        if (existingDestionFolder != null)
        {
            await MergeSourceIntoDestinationFolderAsync(dbVersion: updatedFolder, existingDestionFolder: existingDestionFolder, authorize: authorize);
        }

        Folder destinationFolder = ((existingDestionFolder != null) ? service.GetForUpdate(folderId: existingDestionFolder.Id, ignoreFilters: true) : updatedFolder);

        if (existingDestionFolder == null)
        {
            destinationFolder = authorize
                ? await service.UpdateFolderAsync(updatedFolder: destinationFolder)
                : await service.UpdateForAppFolderAsync(updatedFolder: destinationFolder);
        }

        await UpdateChildrenFolderAsync(updatedFolder: folder, dbVersion: destinationFolder, authorize: authorize);

        if (existingDestionFolder != null)
        {
            if (authorize)
            {
                await service.DeleteAsync(folderId: updatedFolder.Id);
            }
            else
            {
                await service.DeleteAllForAppFolderAsync(deletedFolder: [updatedFolder]);
            }
        }

        return destinationFolder;
    }

    private async ValueTask MergeSourceIntoDestinationFolderAsync(
        Folder dbVersion,
        Folder existingDestionFolder,
        bool authorize)
    {
        if (dbVersion.Files != null && dbVersion.Files.Any())
        {
            foreach (cCoder.Data.Models.DMS.File file in dbVersion.Files)
            {
                file.FolderId = existingDestionFolder.Id;

                file.Folder = new Folder
                {
                    Id = existingDestionFolder.Id,
                    AppId = existingDestionFolder.AppId,
                    Name = existingDestionFolder.Name,
                    Path = existingDestionFolder.Path
                };

                file.RecomputePath();

                if (authorize)
                {
                    await fileService.UpdateFileAsync(updatedFile: file);
                }
                else
                {
                    await fileService.UpdateForAppFileAsync(updatedFile: file);
                }
            }
        }

        if (dbVersion.SubFolders == null || !dbVersion.SubFolders.Any())
        {
            return;
        }

        foreach (Folder subFolder in dbVersion.SubFolders)
        {
            subFolder.ParentId = existingDestionFolder.Id;

            subFolder.Parent = new Folder
            {
                Id = existingDestionFolder.Id,
                AppId = existingDestionFolder.AppId,
                Name = existingDestionFolder.Name,
                Path = existingDestionFolder.Path
            };

            subFolder.RecomputePaths();

            if (authorize)
            {
                await service.UpdateFolderAsync(updatedFolder: subFolder);
            }
            else
            {
                await service.UpdateForAppFolderAsync(updatedFolder: subFolder);
            }
        }
    }

    private async ValueTask UpdateChildrenFolderAsync(Folder updatedFolder, Folder dbVersion, bool authorize)
    {
        if (dbVersion.Files != null && dbVersion.Files.Any())
        {
            foreach (cCoder.Data.Models.DMS.File file in dbVersion.Files)
            {
                file.FolderId = dbVersion.Id;

                file.Folder = new Folder
                {
                    Id = dbVersion.Id,
                    AppId = dbVersion.AppId,
                    Name = dbVersion.Name,
                    Path = dbVersion.Path
                };

                file.RecomputePath();

                if (authorize)
                {
                    await fileService.UpdateFileAsync(updatedFile: file);
                }
                else
                {
                    await fileService.UpdateForAppFileAsync(updatedFile: file);
                }
            }
        }

        if (updatedFolder.Roles != null && updatedFolder.Roles.Any())
        {
            FolderRole[] array = dbVersion.Roles?.ToArray() ?? Array.Empty<FolderRole>();

            foreach (FolderRole existingRole in array)
            {
                await folderRoleService.DeleteFolderRoleAsync(deletedFolderRole: existingRole);
            }

            dbVersion.Roles = new List<FolderRole>();

            foreach (FolderRole role in updatedFolder.Roles)
            {
                FolderRole addedRole = await folderRoleService.AddFolderRoleAsync(newFolderRole: new FolderRole
                {
                    FolderId = dbVersion.Id,
                    RoleId = role.RoleId,
                    Folder = dbVersion,
                    Role = role.Role
                });

                dbVersion.Roles.Add(item: addedRole);
            }
        }

        if (dbVersion.SubFolders == null)
        {
            return;
        }

        foreach (Folder childFolder in dbVersion.SubFolders)
        {
            childFolder.ParentId = dbVersion.Id;

            childFolder.Parent = new Folder
            {
                Id = dbVersion.Id,
                AppId = dbVersion.AppId,
                Name = dbVersion.Name,
                Path = dbVersion.Path
            };

            childFolder.RecomputePaths();

            Folder folder2 = authorize
                ? (!(childFolder.Id != Guid.Empty) ? await AddFolderAsync(newFolder: childFolder) : await UpdateFolderAsync(updatedFolder: childFolder))
                : (!(childFolder.Id != Guid.Empty) ? await AddForAppFolderAsync(newFolder: childFolder) : await UpdateForAppFolderAsync(updatedFolder: childFolder));

            _ = folder2;
        }
    }

    private async ValueTask<Folder> BuildPathAppAsync(App app, cCoder.DocumentManagement.Models.Path folderPath)
    {
        if (folderPath.Length <= 0)
        {
            return null;
        }

        Folder existingFolder = service.GetByPathWithRoles(appId: app.Id, path: folderPath.Lowered, ignoreFilters: true);

        if (existingFolder == null)
        {
            existingFolder = await CreateFolderAppPathAsync(app: app, folderPath: folderPath);
        }

        return existingFolder;
    }

    private async ValueTask<Folder> CreateFolderAppPathAsync(App app, cCoder.DocumentManagement.Models.Path folderPath)
    {
        Folder folder = ((folderPath.ParentPath.Depth <= 0) ? null : (await BuildPathAppAsync(app: app, folderPath: folderPath.ParentPath)));
        Folder parentFolder = folder;
        bool userCanCreateInApp = GetCurrentUser().IsAdminOfApp(appId: app.Id) && GetCurrentUser().Can(appId: app.Id, operation: "folder_create");
        bool userCanCreateFolderInParentFolder = parentFolder?.UserCan(user: GetCurrentUser(), privilege: "folder_create") ?? false;

        if (!userCanCreateInApp && !userCanCreateFolderInParentFolder)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        List<FolderRole> folderRoles = ((parentFolder != null) ? parentFolder.Roles.Select(selector: (FolderRole folderRole) => new FolderRole
        {
            RoleId = folderRole.RoleId
        })
            .ToList() : (from role in roleService.GetAll(ignoreFilters: true)
                         where role.AppId == app.Id
                         select new FolderRole
                         {
                             RoleId = role.Id
                         }).ToList());

        return await service.AddForPathBuildFolderAsync(newFolder: new Folder
        {
            AppId = app.Id,
            ParentId = parentFolder?.Id,
            Name = folderPath.Name,
            Path = folderPath.Lowered,
            Roles = folderRoles
        });
    }

    private async ValueTask DropFolderAppPathAsync(App app, cCoder.DocumentManagement.Models.Path path)
    {
        Folder folder = service.GetByPathWithRoles(appId: app.Id, path: path.Lowered);

        if (folder == null || !folder.UserCan(user: GetCurrentUser(), privilege: "folder_delete"))
        {
            throw new SecurityException(message: "Access Denied!");
        }

        await service.DeleteAsync(folderId: folder.Id);
    }

    private async ValueTask MoveFolderAppPathAsync(App app, cCoder.DocumentManagement.Models.Path oldPath, cCoder.DocumentManagement.Models.Path newPath)
    {
        Folder folder = (string.IsNullOrEmpty(value: newPath.ParentPath.Lowered) ? null : (await BuildPathAppAsync(app: app, folderPath: newPath.ParentPath)));
        Folder newParent = folder;
        Folder oldParent = ((!string.IsNullOrEmpty(value: oldPath.ParentPath.Lowered)) ? service.GetByPathWithRoles(appId: app.Id, path: oldPath.ParentPath.Lowered) : null);
        bool userIsAdmin = GetCurrentUser().IsAdminOfApp(appId: app.Id) && GetCurrentUser().Can(appId: app.Id, operation: "folder_update");

        if (!userIsAdmin && !(oldParent?.UserCan(user: GetCurrentUser(), privilege: "folder_update") ?? false))
        {
            throw new SecurityException(message: "Access Denied!");
        }

        if (!userIsAdmin && !(newParent?.UserCan(user: GetCurrentUser(), privilege: "folder_update") ?? false))
        {
            throw new SecurityException(message: "Access Denied!");
        }

        Folder folder2 = service.GetByPathWithSubFoldersAndFiles(appId: app.Id, path: oldPath.Lowered);

        if (folder2 == null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        (string Name, cCoder.DocumentManagement.Models.Path OldPath)[] subFolderMoves = folder2.SubFolders?.Select(selector: (Folder subFolder) => (Name: subFolder.Name, new cCoder.DocumentManagement.Models.Path(path: subFolder.Path)))
            .ToArray() ?? Array.Empty<(string, cCoder.DocumentManagement.Models.Path)>();

        folder2.ParentId = newParent?.Id;
        folder2.Parent = newParent;
        folder2.Name = newPath.Name;
        folder2.RecomputePaths();
        await service.UpdateFolderAsync(updatedFolder: folder2);

        if (folder2.Files != null)
        {
            foreach (cCoder.Data.Models.DMS.File file in folder2.Files)
            {
                file.FolderId = folder2.Id;
                file.Folder = folder2;
                file.RecomputePath();
                await fileService.UpdateFileAsync(updatedFile: file);
            }
        }

        (string Name, cCoder.DocumentManagement.Models.Path OldPath)[] array = subFolderMoves;

        for (int num = 0; num < array.Length; num++)
        {
            var (name, oldSubFolderPath) = array[num];
            await MoveFolderAppPathAsync(app: app, oldPath: oldSubFolderPath, newPath: new cCoder.DocumentManagement.Models.Path(path: folder2.Path + "/" + name));
        }
    }

    private async ValueTask CopyFolderAppPathAsync(App app, cCoder.DocumentManagement.Models.Path oldPath, cCoder.DocumentManagement.Models.Path newPath)
    {
        Folder sourceFolder = service.GetByPathWithParentAndRoles(appId: app.Id, path: oldPath.Lowered, ignoreFilters: true);

        if (sourceFolder == null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        Folder oldParent = sourceFolder.Parent;
        bool userIsAdmin = GetCurrentUser().IsAdminOfApp(appId: app.Id) && GetCurrentUser().Can(appId: app.Id, operation: "folder_update");

        if (!userIsAdmin && !(oldParent?.UserCan(user: GetCurrentUser(), privilege: "folder_update") ?? false))
        {
            throw new SecurityException(message: "Access Denied!");
        }

        Folder destinationFolder = await BuildPathAppAsync(app: app, folderPath: newPath);

        if (!userIsAdmin && !(destinationFolder?.UserCan(user: GetCurrentUser(), privilege: "folder_update") ?? false))
        {
            throw new SecurityException(message: "Access Denied!");
        }

        cCoder.Data.Models.DMS.File[] sourceFiles = (from file2 in fileService.GetAll(ignoreFilters: true)
                                                     where file2.FolderId == sourceFolder.Id
                                                     select file2).ToArray();

        cCoder.Data.Models.DMS.File[] array = sourceFiles;

        foreach (cCoder.Data.Models.DMS.File file in array)
        {
            await fileProcessingService.CopyAppPathAsync(app: app, oldPath: new cCoder.DocumentManagement.Models.Path(path: file.Path), newPath: new cCoder.DocumentManagement.Models.Path(path: destinationFolder.Path + "/" + file.Name));
        }

        Folder[] sourceSubFolders = (from folder2 in service.GetAll()
                                     where folder2.ParentId == sourceFolder.Id
                                     select folder2).ToArray();

        Folder[] array2 = sourceSubFolders;

        foreach (Folder folder in array2)
        {
            await CopyFolderAppPathAsync(app: app, oldPath: new cCoder.DocumentManagement.Models.Path(path: folder.Path), newPath: new cCoder.DocumentManagement.Models.Path(path: destinationFolder.Path + "/" + folder.Name));
        }
    }

    private FolderArchiveData LoadFolderArchiveData(int appId, string rootPath, bool ignoreFilters)
    {
        Folder[] source = (from foundFolder in GetAll(ignoreFilters: ignoreFilters)
                           where foundFolder.AppId == appId && (foundFolder.Path == rootPath || foundFolder.Path.StartsWith(value: $"{rootPath}/"))
                           select foundFolder).ToArray();

        Guid[] folderIds = source.Select(selector: (Folder folder) => folder.Id)
            .ToArray();

        cCoder.Data.Models.DMS.File[] source2 = ((folderIds.Length == 0) ? Array.Empty<cCoder.Data.Models.DMS.File>() : (from file in fileService.GetAll(ignoreFilters: ignoreFilters)
                                                                                                                         where ((ReadOnlySpan<Guid>)folderIds).Contains(value: file.FolderId)
                                                                                                                         select file).ToArray());

        Guid[] fileIds = source2.Select(selector: (cCoder.Data.Models.DMS.File file) => file.Id)
            .ToArray();

        FileContent[] source3 = ((fileIds.Length == 0) ? Array.Empty<FileContent>() : (from fileContent in fileContentService.GetAll(ignoreFilters: ignoreFilters)
                                                                                       where ((ReadOnlySpan<Guid>)fileIds).Contains(value: fileContent.FileId)
                                                                                       select fileContent).ToArray());

        return new FolderArchiveData(SubFoldersByParentId: source.ToLookup(keySelector: (Folder folder) => folder.ParentId), FilesByFolderId: source2.ToLookup(keySelector: (cCoder.Data.Models.DMS.File file) => file.FolderId), FileContentsByFileId: source3.ToLookup(keySelector: (FileContent fileContent) => fileContent.FileId));
    }

    private static void AddFolderToZipFileFileContent(ZipArchive zip, Folder newFolder, ILookup<Guid?, Folder> subFoldersByParentId, ILookup<Guid, cCoder.Data.Models.DMS.File> filesByFolderId, ILookup<Guid, FileContent> fileContentsByFileId, string prefix = null, string search = "")
    {
        string text = ((prefix == null) ? (newFolder.Name + "/") : (prefix + newFolder.Name + "/"));
        zip.CreateEntry(entryName: text, compressionLevel: CompressionLevel.Optimal);

        foreach (Folder item in subFoldersByParentId[key: newFolder.Id].OrderBy(keySelector: (Folder folder2) => folder2.Name))
        {
            AddFolderToZipFileFileContent(zip: zip, newFolder: item, subFoldersByParentId: subFoldersByParentId, filesByFolderId: filesByFolderId, fileContentsByFileId: fileContentsByFileId, prefix: text, search: search);
        }

        foreach (cCoder.Data.Models.DMS.File item2 in filesByFolderId[key: newFolder.Id].OrderBy(keySelector: (cCoder.Data.Models.DMS.File file) => file.Name))
        {
            if (string.IsNullOrEmpty(value: search) || text.Contains(value: search))
            {
                AddFileToZipFileContent(zip: zip, newFile: item2, fileContents: fileContentsByFileId[key: item2.Id], prefix: text);
            }
        }
    }

    private static void AddFileToZipFileContent(ZipArchive zip, cCoder.Data.Models.DMS.File newFile, IEnumerable<FileContent> fileContents, string prefix = null)
    {
        string entryName = ((prefix != null) ? (prefix + newFile.Name) : newFile.Name);

        byte[] array = (from fileContent in fileContents
                        orderby fileContent.Version descending
                        select fileContent.RawData).FirstOrDefault();

        if (array == null)
        {
            return;
        }

        using Stream stream = zip.CreateEntry(entryName: entryName, compressionLevel: CompressionLevel.Optimal)
            .Open();

        stream.Write(buffer: array, offset: 0, count: array.Length);
    }
    private Folder GetValue(Guid folderId) =>
        Get(folderId: folderId);

    private IQueryable<Folder> GetAllValue(bool ignoreFilters = false) =>
        GetAll(ignoreFilters: ignoreFilters);

    private ValueTask SaveAppPathValueAsync(
        App app,
        cCoder.DocumentManagement.Models.Path path) =>
        SaveAppPathAsync(app: app, path: path);

    private ValueTask<Folder> AddFolderValueAsync(Folder newFolder) =>
        AddFolderAsync(newFolder: newFolder);

    private ValueTask<Folder> UpdateFolderValueAsync(Folder updatedFolder) =>
        UpdateFolderAsync(updatedFolder: updatedFolder);

    private ValueTask DeleteValueAsync(Guid folderId) =>
        DeleteAsync(folderId: folderId);
}