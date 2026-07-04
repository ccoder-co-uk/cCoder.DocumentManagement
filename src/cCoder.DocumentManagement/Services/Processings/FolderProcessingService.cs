using System.IO.Compression;
using System.Security;
using cCoder.DocumentManagement.Brokers;
using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Foundations;

namespace cCoder.DocumentManagement.Services.Processings;

internal class FolderProcessingService(IFolderService service, IFolderRoleService folderRoleService, IRoleService roleService, IFileService fileService, IFileContentService fileContentService, IFileProcessingService fileProcessingService, IAuthorizationBroker authorizationBroker) : IFolderProcessingService
{
    private sealed record FolderArchiveData(ILookup<Guid?, Folder> SubFoldersByParentId, ILookup<Guid, cCoder.Data.Models.DMS.File> FilesByFolderId, ILookup<Guid, FileContent> FileContentsByFileId);

    private User User => authorizationBroker.GetCurrentUser();

    public Folder Get(Guid id)
    {
        return service.Get(id);
    }

    public IQueryable<Folder> GetAll(bool ignoreFilters = false)
    {
        return service.GetAll(ignoreFilters);
    }

    public async ValueTask<List<Result<Guid?>>> CopyAsync(string source, string destination, int sourceAppId, int destAppId)
    {
        Folder sourceFolder = service.GetByPathWithRolesAndFilesAndContents(sourceAppId, source.ToLower(), ignoreFilters: true);
        Folder destinationFolder = service.GetByPathWithRolesAndFilesAndContents(destAppId, destination.ToLower(), ignoreFilters: true);
        if (sourceFolder == null)
        {
            throw new InvalidOperationException("Source folder doesn't exist.");
        }
        if ((!sourceFolder.UserCan(User, "file_update") || !sourceFolder.UserCan(User, "file_create")) && !User.IsAdminOfApp(destAppId))
        {
            throw new SecurityException("Access Denied!");
        }
        if (destinationFolder == null)
        {
            throw new InvalidOperationException("Destination folder doesn't exist.");
        }
        if ((!destinationFolder.UserCan(User, "file_update") || !destinationFolder.UserCan(User, "file_create")) && !User.IsAdminOfApp(destAppId))
        {
            throw new SecurityException("Access Denied!");
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
            using MemoryStream sourceStream = new MemoryStream(entry.Contents.OrderBy((FileContent k) => k.Version).FirstOrDefault().RawData);
            try
            {
                await fileProcessingService.SaveAsync(destinationApp, new cCoder.DocumentManagement.Models.Path(destinationFolder.Path + "/" + entry.Name), sourceStream);
                results.Add(new Result<Guid?>
                {
                    Item = entry.Id,
                    Success = true,
                    Id = entry.Id.ToString()
                });
            }
            catch (Exception ex)
            {
                Exception ex2 = ex;
                results.Add(new Result<Guid?>
                {
                    Item = null,
                    Success = false,
                    Id = entry.Id.ToString(),
                    Message = ex2.Message
                });
            }
        }
        return results;
    }

    public async ValueTask<Folder> AddAsync(Folder newFolder)
    {
        if (newFolder.ParentId.HasValue)
        {
            Folder parent = Get(newFolder.ParentId.Value);
            if (parent == null)
            {
                throw new SecurityException("Access Denied!");
            }
            newFolder.Path = parent.Path + "/" + newFolder.Name;
        }
        else
        {
            newFolder.Path = newFolder.Name;
        }
        Folder existingFolder = GetAll(ignoreFilters: true).FirstOrDefault((Folder folder) => folder.AppId == newFolder.AppId && folder.Path.ToLower() == newFolder.Path.ToLower());
        if (existingFolder != null)
        {
            return existingFolder;
        }
        App app = new App
        {
            Id = newFolder.AppId
        };
        await SaveAsync(app, new cCoder.DocumentManagement.Models.Path(newFolder.Path));
        return GetAll(ignoreFilters: true).FirstOrDefault((Folder folder) => folder.AppId == newFolder.AppId && folder.Path.ToLower() == newFolder.Path.ToLower());
    }

    private async ValueTask<Folder> AddForAppAsync(Folder newFolder)
    {
        if (newFolder.ParentId.HasValue)
        {
            Folder parent = service.GetWithRoles(newFolder.ParentId.Value, ignoreFilters: true);
            if (parent == null)
            {
                throw new InvalidOperationException("Parent folder doesn't exist.");
            }

            newFolder.Path = parent.Path + "/" + newFolder.Name;
        }
        else if (string.IsNullOrWhiteSpace(newFolder.Path))
        {
            newFolder.Path = newFolder.Name;
        }

        Folder existingFolder = GetAll(ignoreFilters: true)
            .FirstOrDefault(folder =>
                folder.AppId == newFolder.AppId
                && folder.Path.ToLower() == newFolder.Path.ToLower());

        return existingFolder ?? await service.AddForPathBuildAsync(newFolder);
    }

    public async ValueTask DeleteAsync(Guid id)
    {
        Folder folder = service.GetWithRoles(id, ignoreFilters: true);
        if (folder == null || (!User.IsAdminOfApp(folder.AppId) && !folder.UserCan(User, "folder_delete")))
        {
            throw new SecurityException("Access Denied!");
        }
        await service.DeleteAsync(id);
    }

    public async ValueTask<Folder> UpdateAsync(Folder folder)
    {
        Folder dbVersion = service.GetForUpdate(folder.Id, ignoreFilters: true);
        if (dbVersion != null && (User.IsAdminOfApp(dbVersion.AppId) || dbVersion.UserCan(User, "folder_update")))
        {
            return await UpdateInternalAsync(dbVersion, folder, authorize: true);
        }
        throw new SecurityException("Access Denied!");
    }

    private async ValueTask<Folder> UpdateForAppAsync(Folder folder)
    {
        Folder dbVersion = service.GetForUpdate(folder.Id, ignoreFilters: true)
            ?? throw new InvalidOperationException("Folder doesn't exist.");

        return await UpdateInternalAsync(dbVersion, folder, authorize: false);
    }

    public async ValueTask HandleFolderDeleteEventAsync(Folder folder)
    {
        Folder dbFolder = GetAll(ignoreFilters: true).FirstOrDefault((Folder foundFolder) => foundFolder.Id == folder.Id);
        if (dbFolder != null)
        {
            string folderPathPrefix = dbFolder.Path + "/";
            Guid[] folderIds = (from foundFolder in GetAll(ignoreFilters: true)
                                where foundFolder.Path == dbFolder.Path || foundFolder.Path.StartsWith(folderPathPrefix)
                                select foundFolder.Id).ToArray();
            Guid[] fileIds = fileService.GetIdsByFolderIds(folderIds, ignoreFilters: true);
            if (fileIds.Length != 0)
            {
                await fileContentService.DeleteAllForFilesAsync(fileIds);
            }
        }
    }

    public DMSResult GetFilesZipped(App app, IEnumerable<cCoder.DocumentManagement.Models.Path> paths)
    {
        using MemoryStream memoryStream = new MemoryStream();
        using (ZipArchive zip = new ZipArchive(memoryStream, ZipArchiveMode.Create))
        {
            foreach (cCoder.DocumentManagement.Models.Path path in paths)
            {
                if (path.IsToFile)
                {
                    cCoder.Data.Models.DMS.File byPathWithFolderAndContents = fileService.GetByPathWithFolderAndContents(app.Id, path.Lowered);
                    if (byPathWithFolderAndContents == null)
                    {
                        throw new SecurityException("Access Denied!");
                    }
                    AddFileToZip(zip, byPathWithFolderAndContents, byPathWithFolderAndContents.Contents);
                    continue;
                }
                Folder byPath = service.GetByPath(app.Id, path.Lowered);
                if (byPath == null)
                {
                    throw new SecurityException("Access Denied!");
                }
                FolderArchiveData folderArchiveData = LoadFolderArchiveData(app.Id, byPath.Path, ignoreFilters: false);
                AddFolderToZip(zip, byPath, folderArchiveData.SubFoldersByParentId, folderArchiveData.FilesByFolderId, folderArchiveData.FileContentsByFileId);
            }
        }
        return new DMSResult
        {
            MimeType = "application/zip",
            Data = new MemoryStream(memoryStream.ToArray())
        };
    }

    public DMSResult Get(App app, cCoder.DocumentManagement.Models.Path path, string search = "")
    {
        if (path.IsToFile)
        {
            throw new InvalidOperationException("To get a file, use file processing operations.");
        }
        Folder byPath = service.GetByPath(app.Id, path.Lowered);
        if (byPath == null)
        {
            throw new SecurityException("Access Denied!");
        }
        FolderArchiveData folderArchiveData = LoadFolderArchiveData(app.Id, byPath.Path, ignoreFilters: false);
        using MemoryStream memoryStream = new MemoryStream();
        using (ZipArchive zip = new ZipArchive(memoryStream, ZipArchiveMode.Create))
        {
            AddFolderToZip(zip, byPath, folderArchiveData.SubFoldersByParentId, folderArchiveData.FilesByFolderId, folderArchiveData.FileContentsByFileId, null, search);
        }
        return new DMSResult
        {
            MimeType = "application/zip",
            Data = new MemoryStream(memoryStream.ToArray())
        };
    }

    public async ValueTask UnpackAsync(App app, cCoder.DocumentManagement.Models.Path path, Stream content, bool ignoreArchiveRoot = false)
    {
        Folder folder = await BuildPathAsync(app, path);
        if (!User.IsAdminOfApp(app.Id) && !folder.UserCan(User, "file_create"))
        {
            throw new SecurityException("Access Denied!");
        }
        using ZipArchive archive = new ZipArchive(content, ZipArchiveMode.Read);
        ZipArchiveEntry rootEntry = archive.Entries.OrderBy((ZipArchiveEntry zipArchiveEntry) => zipArchiveEntry.FullName.Split('/').Length).First();
        string ignoreSegment = rootEntry.FullName;
        foreach (ZipArchiveEntry entry in archive.Entries)
        {
            using Stream entryStream = entry.Open();
            string destinationPath = (ignoreArchiveRoot ? (path.FullPath + "/" + entry.FullName).Replace(ignoreSegment, "") : (path.FullPath + "/" + entry.FullName));
            if (path.Lowered != destinationPath.ToLower())
            {
                await fileProcessingService.SaveAsync(app, new cCoder.DocumentManagement.Models.Path(destinationPath), entryStream);
            }
        }
    }

    public async ValueTask<IEnumerable<Result<Folder>>> AddOrUpdate(IEnumerable<Folder> items)
    {
        List<Result<Folder>> results = new List<Result<Folder>>();

        foreach (Folder item in items)
        {
            try
            {
                Folder savedItem = item.Id == Guid.Empty ? await AddAsync(item) : await UpdateAsync(item);

                results.Add(new Result<Folder>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id == Guid.Empty ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(new Result<Folder>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    public async ValueTask<IEnumerable<Result<Folder>>> AddOrUpdateForAppAsync(IEnumerable<Folder> items)
    {
        List<Result<Folder>> results = new List<Result<Folder>>();

        foreach (Folder item in items)
        {
            try
            {
                Folder savedItem = item.Id == Guid.Empty
                    ? await AddForAppAsync(item)
                    : await UpdateForAppAsync(item);

                results.Add(new Result<Folder>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id == Guid.Empty ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(new Result<Folder>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    public async ValueTask DeleteAllAsync(IEnumerable<Folder> items)
    {
        foreach (Folder item in items)
        {
            await DeleteAsync(item.Id);
        }
    }

    public async ValueTask DeleteByAppIdAsync(int appId)
    {
        Folder[] folders =
            [.. service.GetAll(ignoreFilters: true)
                .Where(folder => folder.AppId == appId)
                .OrderByDescending(folder => folder.Path.Length)];

        if (folders.Length == 0)
        {
            return;
        }

        Guid[] folderIds = [.. folders.Select(folder => folder.Id)];
        Guid[] fileIds = fileService.GetIdsByFolderIds(folderIds, ignoreFilters: true);

        if (fileIds.Length > 0)
        {
            await fileContentService.DeleteAllForFilesAsync(fileIds);
            await fileService.DeleteAllForAppAsync(
                fileService.GetAll(ignoreFilters: true)
                    .Where(file => fileIds.Contains(file.Id))
                    .ToArray());
        }

        await service.DeleteAllForAppAsync(folders);
    }

    public async ValueTask SaveAsync(App app, cCoder.DocumentManagement.Models.Path path)
    {
        await BuildPathAsync(app, path);
    }

    public async ValueTask DropAsync(App app, cCoder.DocumentManagement.Models.Path path)
    {
        await DropFolderAsync(app, path);
    }

    public async ValueTask CopyAsync(App app, cCoder.DocumentManagement.Models.Path oldPath, cCoder.DocumentManagement.Models.Path newPath)
    {
        if (oldPath.IsToFile)
        {
            throw new InvalidOperationException("To copy a file, use file processing operations.");
        }
        await CopyFolderAsync(app, oldPath, newPath);
    }

    public async ValueTask MoveAsync(App app, cCoder.DocumentManagement.Models.Path oldPath, cCoder.DocumentManagement.Models.Path newPath)
    {
        if (oldPath.IsToFile)
        {
            throw new InvalidOperationException("To move a file, use file processing operations.");
        }
        Folder newParent = ((!string.IsNullOrEmpty(newPath.ParentPath.Lowered)) ? service.GetByPath(app.Id, newPath.ParentPath.Lowered) : null);
        cCoder.DocumentManagement.Models.Path resolvedNewPath = new cCoder.DocumentManagement.Models.Path((newParent != null) ? (newParent.Path + "/" + newPath.Name) : newPath.Name);
        await MoveFolderAsync(app, oldPath, resolvedNewPath);
    }

    private async ValueTask<Folder> UpdateInternalAsync(Folder dbVersion, Folder folder, bool authorize)
    {
        string parentPath = new cCoder.DocumentManagement.Models.Path(folder.Path).ParentPath.FullPath;
        string newPath = ((!string.IsNullOrEmpty(parentPath)) ? "/" : "") + folder.Name.ToLower();
        Folder existingDestionFolder = GetAll().FirstOrDefault((Folder foundFolder) => foundFolder.Path == newPath && foundFolder.Path != dbVersion.Path && foundFolder.AppId == folder.AppId);
        if (folder.ParentId != dbVersion.ParentId)
        {
            dbVersion.Parent = (folder.ParentId.HasValue ? service.Get(folder.ParentId.Value) : null);
        }
        dbVersion.AppId = folder.AppId;
        dbVersion.ParentId = folder.ParentId;
        dbVersion.Name = folder.Name;
        dbVersion.Path = folder.Path;
        dbVersion.DeletedOn = folder.DeletedOn;
        dbVersion.RecomputePaths();
        if (existingDestionFolder != null)
        {
            await MergeSourceIntoDestinationAsync(dbVersion, existingDestionFolder, authorize);
        }
        Folder destinationFolder = ((existingDestionFolder != null) ? service.GetForUpdate(existingDestionFolder.Id, ignoreFilters: true) : dbVersion);
        if (existingDestionFolder == null)
        {
            destinationFolder = authorize
                ? await service.UpdateAsync(destinationFolder)
                : await service.UpdateForAppAsync(destinationFolder);
        }
        await UpdateChildrenAsync(folder, destinationFolder, authorize);
        if (existingDestionFolder != null)
        {
            if (authorize)
                await service.DeleteAsync(dbVersion.Id);
            else
                await service.DeleteAllForAppAsync([dbVersion]);
        }
        return destinationFolder;
    }

    private async ValueTask MergeSourceIntoDestinationAsync(
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
                    await fileService.UpdateAsync(file);
                else
                    await fileService.UpdateForAppAsync(file);
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
                await service.UpdateAsync(subFolder);
            else
                await service.UpdateForAppAsync(subFolder);
        }
    }

    private async ValueTask UpdateChildrenAsync(Folder folder, Folder dbVersion, bool authorize)
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
                    await fileService.UpdateAsync(file);
                else
                    await fileService.UpdateForAppAsync(file);
            }
        }
        if (folder.Roles != null && folder.Roles.Any())
        {
            FolderRole[] array = dbVersion.Roles?.ToArray() ?? Array.Empty<FolderRole>();
            foreach (FolderRole existingRole in array)
            {
                await folderRoleService.DeleteAsync(existingRole);
            }
            dbVersion.Roles = new List<FolderRole>();
            foreach (FolderRole role in folder.Roles)
            {
                FolderRole addedRole = await folderRoleService.AddAsync(new FolderRole
                {
                    FolderId = dbVersion.Id,
                    RoleId = role.RoleId,
                    Folder = dbVersion,
                    Role = role.Role
                });
                dbVersion.Roles.Add(addedRole);
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
                ? (!(childFolder.Id != Guid.Empty) ? await AddAsync(childFolder) : await UpdateAsync(childFolder))
                : (!(childFolder.Id != Guid.Empty) ? await AddForAppAsync(childFolder) : await UpdateForAppAsync(childFolder));
            _ = folder2;
        }
    }

    private async ValueTask<Folder> BuildPathAsync(App app, cCoder.DocumentManagement.Models.Path folderPath)
    {
        if (folderPath.Length <= 0)
        {
            return null;
        }
        Folder existingFolder = service.GetByPathWithRoles(app.Id, folderPath.Lowered, ignoreFilters: true);
        if (existingFolder == null)
        {
            existingFolder = await CreateFolderAsync(app, folderPath);
        }
        return existingFolder;
    }

    private async ValueTask<Folder> CreateFolderAsync(App app, cCoder.DocumentManagement.Models.Path folderPath)
    {
        Folder folder = ((folderPath.ParentPath.Depth <= 0) ? null : (await BuildPathAsync(app, folderPath.ParentPath)));
        Folder parentFolder = folder;
        bool userCanCreateInApp = User.IsAdminOfApp(app.Id) && User.Can(app.Id, "folder_create");
        bool userCanCreateFolderInParentFolder = parentFolder?.UserCan(User, "folder_create") ?? false;
        if (!userCanCreateInApp && !userCanCreateFolderInParentFolder)
        {
            throw new SecurityException("Access Denied!");
        }
        List<FolderRole> folderRoles = ((parentFolder != null) ? parentFolder.Roles.Select((FolderRole folderRole) => new FolderRole
        {
            RoleId = folderRole.RoleId
        }).ToList() : (from role in roleService.GetAll(ignoreFilters: true)
                       where role.AppId == app.Id
                       select new FolderRole
                       {
                           RoleId = role.Id
                       }).ToList());
        return await service.AddForPathBuildAsync(new Folder
        {
            AppId = app.Id,
            ParentId = parentFolder?.Id,
            Name = folderPath.Name,
            Path = folderPath.Lowered,
            Roles = folderRoles
        });
    }

    private async ValueTask DropFolderAsync(App app, cCoder.DocumentManagement.Models.Path path)
    {
        Folder folder = service.GetByPathWithRoles(app.Id, path.Lowered);
        if (folder == null || !folder.UserCan(User, "folder_delete"))
        {
            throw new SecurityException("Access Denied!");
        }
        await service.DeleteAsync(folder.Id);
    }

    private async ValueTask MoveFolderAsync(App app, cCoder.DocumentManagement.Models.Path oldPath, cCoder.DocumentManagement.Models.Path newPath)
    {
        Folder folder = (string.IsNullOrEmpty(newPath.ParentPath.Lowered) ? null : (await BuildPathAsync(app, newPath.ParentPath)));
        Folder newParent = folder;
        Folder oldParent = ((!string.IsNullOrEmpty(oldPath.ParentPath.Lowered)) ? service.GetByPathWithRoles(app.Id, oldPath.ParentPath.Lowered) : null);
        bool userIsAdmin = User.IsAdminOfApp(app.Id) && User.Can(app.Id, "folder_update");
        if (!userIsAdmin && !(oldParent?.UserCan(User, "folder_update") ?? false))
        {
            throw new SecurityException("Access Denied!");
        }
        if (!userIsAdmin && !(newParent?.UserCan(User, "folder_update") ?? false))
        {
            throw new SecurityException("Access Denied!");
        }
        Folder folder2 = service.GetByPathWithSubFoldersAndFiles(app.Id, oldPath.Lowered);
        if (folder2 == null)
        {
            throw new SecurityException("Access Denied!");
        }
        (string Name, cCoder.DocumentManagement.Models.Path OldPath)[] subFolderMoves = folder2.SubFolders?.Select((Folder subFolder) => (Name: subFolder.Name, new cCoder.DocumentManagement.Models.Path(subFolder.Path))).ToArray() ?? Array.Empty<(string, cCoder.DocumentManagement.Models.Path)>();
        folder2.ParentId = newParent?.Id;
        folder2.Parent = newParent;
        folder2.Name = newPath.Name;
        folder2.RecomputePaths();
        await service.UpdateAsync(folder2);
        if (folder2.Files != null)
        {
            foreach (cCoder.Data.Models.DMS.File file in folder2.Files)
            {
                file.FolderId = folder2.Id;
                file.Folder = folder2;
                file.RecomputePath();
                await fileService.UpdateAsync(file);
            }
        }
        (string Name, cCoder.DocumentManagement.Models.Path OldPath)[] array = subFolderMoves;
        for (int num = 0; num < array.Length; num++)
        {
            var (name, oldSubFolderPath) = array[num];
            await MoveFolderAsync(app, oldSubFolderPath, new cCoder.DocumentManagement.Models.Path(folder2.Path + "/" + name));
        }
    }

    private async ValueTask CopyFolderAsync(App app, cCoder.DocumentManagement.Models.Path oldPath, cCoder.DocumentManagement.Models.Path newPath)
    {
        Folder sourceFolder = service.GetByPathWithParentAndRoles(app.Id, oldPath.Lowered, ignoreFilters: true);
        if (sourceFolder == null)
        {
            throw new SecurityException("Access Denied!");
        }
        Folder oldParent = sourceFolder.Parent;
        bool userIsAdmin = User.IsAdminOfApp(app.Id) && User.Can(app.Id, "folder_update");
        if (!userIsAdmin && !(oldParent?.UserCan(User, "folder_update") ?? false))
        {
            throw new SecurityException("Access Denied!");
        }
        Folder destinationFolder = await BuildPathAsync(app, newPath);
        if (!userIsAdmin && !(destinationFolder?.UserCan(User, "folder_update") ?? false))
        {
            throw new SecurityException("Access Denied!");
        }
        cCoder.Data.Models.DMS.File[] sourceFiles = (from file2 in fileService.GetAll(ignoreFilters: true)
                                                               where file2.FolderId == sourceFolder.Id
                                                               select file2).ToArray();
        cCoder.Data.Models.DMS.File[] array = sourceFiles;
        foreach (cCoder.Data.Models.DMS.File file in array)
        {
            await fileProcessingService.CopyAsync(app, new cCoder.DocumentManagement.Models.Path(file.Path), new cCoder.DocumentManagement.Models.Path(destinationFolder.Path + "/" + file.Name));
        }
        Folder[] sourceSubFolders = (from folder2 in service.GetAll()
                                     where folder2.ParentId == sourceFolder.Id
                                     select folder2).ToArray();
        Folder[] array2 = sourceSubFolders;
        foreach (Folder folder in array2)
        {
            await CopyFolderAsync(app, new cCoder.DocumentManagement.Models.Path(folder.Path), new cCoder.DocumentManagement.Models.Path(destinationFolder.Path + "/" + folder.Name));
        }
    }

    private FolderArchiveData LoadFolderArchiveData(int appId, string rootPath, bool ignoreFilters)
    {
        Folder[] source = (from foundFolder in GetAll(ignoreFilters)
                           where foundFolder.AppId == appId && (foundFolder.Path == rootPath || foundFolder.Path.StartsWith($"{rootPath}/"))
                           select foundFolder).ToArray();
        Guid[] folderIds = source.Select((Folder folder) => folder.Id).ToArray();
        cCoder.Data.Models.DMS.File[] source2 = ((folderIds.Length == 0) ? Array.Empty<cCoder.Data.Models.DMS.File>() : (from file in fileService.GetAll(ignoreFilters)
                                                                                                                                             where ((ReadOnlySpan<Guid>)folderIds).Contains(file.FolderId)
                                                                                                                                             select file).ToArray());
        Guid[] fileIds = source2.Select((cCoder.Data.Models.DMS.File file) => file.Id).ToArray();
        FileContent[] source3 = ((fileIds.Length == 0) ? Array.Empty<FileContent>() : (from fileContent in fileContentService.GetAll(ignoreFilters)
                                                                                       where ((ReadOnlySpan<Guid>)fileIds).Contains(fileContent.FileId)
                                                                                       select fileContent).ToArray());
        return new FolderArchiveData(source.ToLookup((Folder folder) => folder.ParentId), source2.ToLookup((cCoder.Data.Models.DMS.File file) => file.FolderId), source3.ToLookup((FileContent fileContent) => fileContent.FileId));
    }

    private static void AddFolderToZip(ZipArchive zip, Folder folder, ILookup<Guid?, Folder> subFoldersByParentId, ILookup<Guid, cCoder.Data.Models.DMS.File> filesByFolderId, ILookup<Guid, FileContent> fileContentsByFileId, string prefix = null, string search = "")
    {
        string text = ((prefix == null) ? (folder.Name + "/") : (prefix + folder.Name + "/"));
        zip.CreateEntry(text, CompressionLevel.Optimal);
        foreach (Folder item in subFoldersByParentId[folder.Id].OrderBy((Folder folder2) => folder2.Name))
        {
            AddFolderToZip(zip, item, subFoldersByParentId, filesByFolderId, fileContentsByFileId, text, search);
        }
        foreach (cCoder.Data.Models.DMS.File item2 in filesByFolderId[folder.Id].OrderBy((cCoder.Data.Models.DMS.File file) => file.Name))
        {
            if (string.IsNullOrEmpty(search) || text.Contains(search))
            {
                AddFileToZip(zip, item2, fileContentsByFileId[item2.Id], text);
            }
        }
    }

    private static void AddFileToZip(ZipArchive zip, cCoder.Data.Models.DMS.File file, IEnumerable<FileContent> fileContents, string prefix = null)
    {
        string entryName = ((prefix != null) ? (prefix + file.Name) : file.Name);
        byte[] array = (from fileContent in fileContents
                        orderby fileContent.Version descending
                        select fileContent.RawData).FirstOrDefault();
        if (array == null)
        {
            return;
        }
        using Stream stream = zip.CreateEntry(entryName, CompressionLevel.Optimal).Open();
        stream.Write(array, 0, array.Length);
    }
}
