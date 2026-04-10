using System.Security;
using System.Text;
using cCoder.DocumentManagement.Brokers;
using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Foundations;

namespace cCoder.DocumentManagement.Services.Processings;

internal class FileProcessingService(IFileService service, IFolderService folderService, IFileContentService fileContentService, IFileContentProcessingService fileContentProcessingService, IAuthorizationBroker authorizationBroker) : IFileProcessingService
{
    private User User => authorizationBroker.GetCurrentUser();

    public cCoder.Data.Models.DMS.File Get(Guid id)
    {
        return service.Get(id);
    }

    public IQueryable<cCoder.Data.Models.DMS.File> GetAll(bool ignoreFilters = false)
    {
        return service.GetAll(ignoreFilters);
    }

    public async ValueTask<cCoder.Data.Models.DMS.File> AddAsync(cCoder.Data.Models.DMS.File newFile)
    {
        Folder folder = folderService.GetWithRoles(newFile.FolderId, ignoreFilters: true);
        if (folder == null)
        {
            throw new SecurityException("Access Denied!");
        }
        if (!User.IsAdminOfApp(folder.AppId) && !folder.UserCan(User, "file_create"))
        {
            throw new SecurityException("Access Denied!");
        }
        string relativePath = (string.IsNullOrWhiteSpace(newFile.Path) ? newFile.Name : newFile.Path);
        string fileName = (string.IsNullOrWhiteSpace(newFile.Name) ? new cCoder.DocumentManagement.Models.Path(relativePath).Name : newFile.Name);
        cCoder.Data.Models.DMS.File createdFile = await service.AddAsync(new cCoder.Data.Models.DMS.File
        {
            FolderId = folder.Id,
            Folder = folder,
            Name = fileName,
            Description = newFile.Description,
            Path = (folder.Path + "/" + relativePath).Trim('/').ToLowerInvariant(),
            MimeType = newFile.MimeType,
            Size = newFile.Size
        });
        if (newFile.Contents != null && newFile.Contents.Any())
        {
            FileContent[] contents = newFile.Contents.Select((FileContent content) => new FileContent
            {
                Id = content.Id,
                FileId = createdFile.Id,
                File = createdFile,
                Description = content.Description,
                Size = content.Size,
                Version = content.Version,
                RawData = content.RawData
            }).ToArray();
            await fileContentProcessingService.AddOrUpdate(contents);
        }
        return service.GetWithFolderAndContents(createdFile.Id, ignoreFilters: true);
    }

    public cCoder.Data.Models.DMS.File GetByPath(int appId, string path)
    {
        cCoder.Data.Models.DMS.File byPath = service.GetByPath(appId, path, ignoreFilters: true);
        if (byPath != null)
        {
            return byPath;
        }
        throw new SecurityException("Access Denied!");
    }

    public ValueTask DeleteAsync(Guid id)
    {
        return service.DeleteAsync(id);
    }

    public async ValueTask<cCoder.Data.Models.DMS.File> UpdateAsync(cCoder.Data.Models.DMS.File newFile)
    {
        cCoder.Data.Models.DMS.File dbVersion = service.GetWithFolderRolesAndContents(newFile.Id, ignoreFilters: true);
        if (dbVersion == null || !dbVersion.UserCan(User, "file_update"))
        {
            throw new SecurityException("Access Denied!");
        }
        dbVersion.Description = newFile.Description;
        dbVersion.Size = newFile.Size;
        dbVersion.MimeType = newFile.MimeType;
        if (dbVersion.Name != newFile.Name || dbVersion.FolderId != newFile.FolderId)
        {
            Guid originalFolderId = dbVersion.FolderId;
            dbVersion.Name = newFile.Name;
            dbVersion.FolderId = newFile.FolderId;
            dbVersion.Folder = ((newFile.FolderId == originalFolderId) ? dbVersion.Folder : folderService.GetWithRoles(newFile.FolderId, ignoreFilters: true));
            dbVersion.RecomputePath();
        }
        cCoder.Data.Models.DMS.File updatedFile = await service.UpdateAsync(dbVersion);
        if (newFile.Contents != null && newFile.Contents.Any())
        {
            FileContent[] contents = newFile.Contents.Select((FileContent content) => new FileContent
            {
                Id = content.Id,
                FileId = updatedFile.Id,
                File = updatedFile,
                Description = content.Description,
                Size = content.Size,
                CreatedBy = content.CreatedBy,
                CreatedOn = content.CreatedOn,
                Version = content.Version,
                RawData = content.RawData
            }).ToArray();
            await fileContentProcessingService.AddOrUpdate(contents);
        }
        return service.GetWithFolderAndContents(updatedFile.Id, ignoreFilters: true);
    }

    public ValueTask HandleFileDeleteEventAsync(cCoder.Data.Models.DMS.File file)
    {
        return fileContentService.DeleteAllForFileAsync(file.Id);
    }

    public DMSResult Get(App app, cCoder.DocumentManagement.Models.Path path, int version = 0)
    {
        if (!path.IsToFile)
        {
            throw new InvalidOperationException("To get a folder archive, use folder processing operations.");
        }
        cCoder.Data.Models.DMS.File byPathWithFolderAndContents = service.GetByPathWithFolderAndContents(app.Id, path.Lowered);
        if (byPathWithFolderAndContents == null)
        {
            throw new SecurityException("Access Denied!");
        }
        return new DMSResult
        {
            MimeType = byPathWithFolderAndContents.MimeType,
            Data = byPathWithFolderAndContents.GetContent(version)
        };
    }

    public IEnumerable<cCoder.Data.Models.DMS.File> Search(App app, string needle)
    {
        return service.Search(app.Id, Encoding.UTF8.GetBytes(needle)).AsEnumerable();
    }

    public async ValueTask SaveAsync(App app, cCoder.DocumentManagement.Models.Path path, Stream content = null)
    {
        if (path.IsToFile)
        {
            cCoder.Data.Models.DMS.File existingFile = service.GetByPath(app.Id, path.Lowered);
            byte[] rawBytes = ReadAllBytes(content);
            Folder folder = await BuildPathAsync(app, path.ParentPath);
            if (existingFile == null)
            {
                await CreateNewFileAsync(app, path, rawBytes, folder);
            }
            else
            {
                await UpdateFileAsync(app, existingFile, rawBytes, folder);
            }
        }
        else
        {
            await BuildPathAsync(app, path);
        }
    }

    public async ValueTask DropAsync(App app, cCoder.DocumentManagement.Models.Path path, int version = 0)
    {
        if (path.IsToFile)
        {
            await DropFileAsync(app, path, version);
            return;
        }
        throw new InvalidOperationException("To delete a folder, use folder processing operations.");
    }

    public async ValueTask CopyAsync(App app, cCoder.DocumentManagement.Models.Path oldPath, cCoder.DocumentManagement.Models.Path newPath)
    {
        if (!oldPath.IsToFile)
        {
            throw new InvalidOperationException("To copy a folder, use folder processing operations.");
        }
        await CopyFileAsync(app, oldPath, newPath);
    }

    public async ValueTask MoveAsync(App app, cCoder.DocumentManagement.Models.Path oldPath, cCoder.DocumentManagement.Models.Path newPath)
    {
        if (!oldPath.IsToFile)
        {
            throw new InvalidOperationException("To move a folder, use folder processing operations.");
        }
        Folder newParent = ((!string.IsNullOrEmpty(newPath.ParentPath.Lowered)) ? folderService.GetByPathWithRoles(app.Id, newPath.ParentPath.Lowered) : null);
        Folder oldParent = ((!string.IsNullOrEmpty(oldPath.ParentPath.Lowered)) ? folderService.GetByPathWithRoles(app.Id, oldPath.ParentPath.Lowered) : null);
        bool userIsAdmin = User.IsAdminOfApp(app.Id);
        if (newParent == null && !newPath.IsToFile)
        {
            newParent = await BuildPathAsync(app, newPath);
        }
        if (newParent == null && newPath.IsToFile)
        {
            newParent = await BuildPathAsync(app, newPath.ParentPath);
        }
        await MoveFileAsync(app, oldPath, newPath, newParent, oldParent, userIsAdmin);
    }

    public async ValueTask<IEnumerable<Result<cCoder.Data.Models.DMS.File>>> AddOrUpdate(IEnumerable<cCoder.Data.Models.DMS.File> items)
    {
        List<Result<cCoder.Data.Models.DMS.File>> results = new List<Result<cCoder.Data.Models.DMS.File>>();

        foreach (cCoder.Data.Models.DMS.File item in items)
        {
            try
            {
                cCoder.Data.Models.DMS.File savedItem = item.Id == Guid.Empty ? await AddAsync(item) : await UpdateAsync(item);

                results.Add(new Result<cCoder.Data.Models.DMS.File>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id == Guid.Empty ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(new Result<cCoder.Data.Models.DMS.File>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    public async ValueTask DeleteAllAsync(IEnumerable<cCoder.Data.Models.DMS.File> items)
    {
        foreach (cCoder.Data.Models.DMS.File item in items)
        {
            await DeleteAsync(item.Id);
        }
    }

    private async ValueTask UpdateFileAsync(App app, cCoder.Data.Models.DMS.File existingFile, byte[] rawBytes, Folder folder)
    {
        if (!User.IsAdminOfApp(app.Id) && !folder.UserCan(User, "file_update"))
        {
            throw new SecurityException("Access Denied!");
        }
        await SaveFileVersionAsync(existingFile, rawBytes);
    }

    private async ValueTask CreateNewFileAsync(App app, cCoder.DocumentManagement.Models.Path path, byte[] rawBytes, Folder folder)
    {
        if (!User.IsAdminOfApp(app.Id) && !folder.UserCan(User, "file_create"))
        {
            throw new SecurityException("Access Denied!");
        }
        await CreateFileAsync(path, rawBytes, folder);
    }

    private async ValueTask SaveFileVersionAsync(cCoder.Data.Models.DMS.File existingFile, byte[] rawBytes)
    {
        int version = (from fileContent in fileContentProcessingService.GetAll()
                       where fileContent.FileId == existingFile.Id
                       orderby fileContent.Version descending
                       select fileContent.Version).First() + 1;
        await fileContentProcessingService.AddAsync(new FileContent
        {
            CreatedBy = User.Id,
            CreatedOn = DateTimeOffset.UtcNow,
            FileId = existingFile.Id,
            Version = version,
            Size = GetSizeOf(rawBytes),
            RawData = rawBytes,
            File = existingFile
        });
    }

    private async ValueTask CreateFileAsync(cCoder.DocumentManagement.Models.Path path, byte[] rawBytes, Folder folder)
    {
        cCoder.Data.Models.DMS.File fileObject = await service.AddAsync(new cCoder.Data.Models.DMS.File
        {
            CreatedBy = User.Id,
            CreatedOn = DateTimeOffset.UtcNow,
            Name = path.Name,
            Path = path.Lowered,
            FolderId = folder.Id,
            Folder = folder,
            MimeType = path.MimeType,
            Size = GetSizeOf(rawBytes)
        });
        await fileContentProcessingService.AddAsync(new FileContent
        {
            CreatedBy = User.Id,
            CreatedOn = DateTimeOffset.UtcNow,
            FileId = fileObject.Id,
            File = fileObject,
            Version = 1,
            Size = GetSizeOf(rawBytes),
            RawData = rawBytes
        });
    }

    private async ValueTask DropFileAsync(App app, cCoder.DocumentManagement.Models.Path path, int version)
    {
        cCoder.Data.Models.DMS.File file = service.GetByPathWithFolderRolesAndContents(app.Id, path.Lowered, ignoreFilters: true);
        if (file == null || !file.UserCan(User, "file_delete"))
        {
            throw new SecurityException("Access Denied!");
        }
        if (version != 0)
        {
            await DropFileVersionAsync(version, file);
        }
        else
        {
            await service.DeleteAsync(file.Id);
        }
    }

    private async ValueTask DropFileVersionAsync(int version, cCoder.Data.Models.DMS.File file)
    {
        FileContent versionedContent = fileContentProcessingService.GetAll(ignoreFilters: true).FirstOrDefault((FileContent fileContent) => fileContent.FileId == file.Id && fileContent.Version == version);
        if (versionedContent == null)
        {
            throw new SecurityException("Access Denied!");
        }
        await fileContentProcessingService.DeleteAsync(versionedContent.Id);
        if (!fileContentProcessingService.GetAll(ignoreFilters: true).Any((FileContent fileContent) => fileContent.FileId == file.Id))
        {
            await service.DeleteAsync(file.Id);
        }
    }

    private async ValueTask MoveFileAsync(App app, cCoder.DocumentManagement.Models.Path oldPath, cCoder.DocumentManagement.Models.Path newPath, Folder newParent, Folder oldParent, bool userIsAdmin)
    {
        ConfirmUserCanMoveFile(oldPath, newPath, newParent, oldParent, userIsAdmin);
        cCoder.Data.Models.DMS.File sourceFile = service.GetByPathWithFolderRolesAndContents(app.Id, oldPath.Lowered, ignoreFilters: true);
        if (sourceFile == null)
        {
            throw new SecurityException("Access Denied!");
        }
        cCoder.Data.Models.DMS.File destinationFile = service.GetByPathWithFolderAndContents(app.Id, newPath.Lowered, ignoreFilters: true);
        if (destinationFile != null)
        {
            int latestContentVersion = (destinationFile.Contents.Any() ? destinationFile.Contents.Max((FileContent fileContent) => fileContent.Version) : 0);
            FileContent[] copiedContents = sourceFile.Contents.Select((FileContent content) => new FileContent
            {
                FileId = destinationFile.Id,
                File = destinationFile,
                Description = content.Description,
                Size = content.Size,
                CreatedBy = content.CreatedBy,
                CreatedOn = content.CreatedOn,
                Version = content.Version + latestContentVersion,
                RawData = content.RawData
            }).ToArray();
            await fileContentProcessingService.AddOrUpdate(copiedContents);
            await DropAsync(app, oldPath);
        }
        else if (!newPath.IsToFile)
        {
            Folder newPathFolder = await BuildPathAsync(app, newPath);
            await MoveFileAsync(app, oldPath, new cCoder.DocumentManagement.Models.Path(newPathFolder.Path + "/" + oldPath.Name), newPathFolder, oldParent, userIsAdmin);
        }
        else
        {
            sourceFile.FolderId = newParent.Id;
            sourceFile.Folder = newParent;
            sourceFile.Name = newPath.Name;
            sourceFile.Path = (newParent.Path + "/" + newPath.Name).ToLower();
            await service.UpdateAsync(sourceFile);
        }
    }

    private async ValueTask CopyFileAsync(App app, cCoder.DocumentManagement.Models.Path oldPath, cCoder.DocumentManagement.Models.Path newPath)
    {
        Folder newParent = ((!string.IsNullOrEmpty(newPath.ParentPath.Lowered)) ? folderService.GetByPathWithRoles(app.Id, newPath.ParentPath.Lowered) : null);
        Folder oldParent = ((!string.IsNullOrEmpty(oldPath.ParentPath.Lowered)) ? folderService.GetByPathWithRoles(app.Id, oldPath.ParentPath.Lowered) : null);
        bool userIsAdmin = User.IsAdminOfApp(app.Id);
        if (newParent == null && !newPath.IsToFile)
        {
            newParent = await BuildPathAsync(app, newPath);
        }
        if (newParent == null && newPath.IsToFile)
        {
            newParent = await BuildPathAsync(app, newPath.ParentPath);
        }
        ConfirmUserCanMoveFile(oldPath, newPath, newParent, oldParent, userIsAdmin);
        cCoder.Data.Models.DMS.File sourceFile = service.GetByPathWithFolderRolesAndContents(app.Id, oldPath.Lowered, ignoreFilters: true);
        if (sourceFile == null)
        {
            throw new SecurityException("Access Denied!");
        }
        if (!newPath.IsToFile)
        {
            await CopyFileAsync(app, oldPath, new cCoder.DocumentManagement.Models.Path((await BuildPathAsync(app, newPath)).Path + "/" + oldPath.Name));
            return;
        }
        cCoder.Data.Models.DMS.File destinationFile = service.GetByPathWithFolderAndContents(app.Id, newPath.Lowered, ignoreFilters: true);
        if (destinationFile != null)
        {
            int latestContentVersion = (destinationFile.Contents.Any() ? destinationFile.Contents.Max((FileContent fileContent) => fileContent.Version) : 0);
            FileContent[] copiedContents = (from content in sourceFile.Contents
                                            orderby content.Version
                                            select new FileContent
                                            {
                                                FileId = destinationFile.Id,
                                                File = destinationFile,
                                                CreatedBy = content.CreatedBy,
                                                CreatedOn = content.CreatedOn,
                                                Description = content.Description,
                                                RawData = content.RawData,
                                                Size = content.Size,
                                                Version = content.Version + latestContentVersion
                                            }).ToArray();
            await fileContentProcessingService.AddOrUpdate(copiedContents);
            return;
        }
        cCoder.Data.Models.DMS.File copiedFile = await service.AddAsync(new cCoder.Data.Models.DMS.File
        {
            CreatedBy = sourceFile.CreatedBy,
            CreatedOn = sourceFile.CreatedOn,
            Name = newPath.Name,
            Path = (newParent.Path + "/" + newPath.Name).ToLower(),
            FolderId = newParent.Id,
            Folder = newParent,
            MimeType = sourceFile.MimeType,
            Description = sourceFile.Description,
            Size = sourceFile.Size
        });
        FileContent[] copiedFileContents = (from content in sourceFile.Contents
                                            orderby content.Version
                                            select new FileContent
                                            {
                                                FileId = copiedFile.Id,
                                                File = copiedFile,
                                                CreatedBy = content.CreatedBy,
                                                CreatedOn = content.CreatedOn,
                                                Description = content.Description,
                                                RawData = content.RawData,
                                                Size = content.Size,
                                                Version = content.Version
                                            }).ToArray();
        await fileContentProcessingService.AddOrUpdate(copiedFileContents);
    }

    private void ConfirmUserCanMoveFile(cCoder.DocumentManagement.Models.Path oldPath, cCoder.DocumentManagement.Models.Path newPath, Folder newParent, Folder oldParent, bool userIsAdmin)
    {
        if (!userIsAdmin && !(oldParent?.UserCan(User, "file_update") ?? false))
        {
            throw new SecurityException("Access Denied!");
        }
        if (!userIsAdmin && !(newParent?.UserCan(User, "file_update") ?? false))
        {
            throw new SecurityException("Access Denied!");
        }
    }

    private async ValueTask<Folder> BuildPathAsync(App app, cCoder.DocumentManagement.Models.Path folderPath)
    {
        if (folderPath.Length <= 0)
        {
            return null;
        }
        Folder existingFolder = folderService.GetByPathWithRoles(app.Id, folderPath.Lowered);
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
        }).ToList() : new List<FolderRole>());
        Folder folder2 = folderService.GetByPath(app.Id, folderPath.Lowered) ?? new Folder
        {
            Id = Guid.Empty,
            AppId = app.Id,
            Name = folderPath.Name,
            Parent = parentFolder,
            ParentId = parentFolder?.Id,
            Path = folderPath.Lowered,
            Roles = folderRoles
        };
        if (folder2.Id == Guid.Empty)
        {
            folder2 = await folderService.AddAsync(folder2);
        }
        return folder2;
    }

    private static string GetSizeOf(byte[] content)
    {
        if (content.Length > 1000000000)
        {
            return $"{content.Length / 1000 / 1000 / 1000} GB";
        }
        if (content.Length > 1000000)
        {
            return $"{content.Length / 1000 / 1000} MB";
        }
        return (content.Length > 1000) ? $"{content.Length / 1000} KB" : $"{content.Length} B";
    }

    private static byte[] ReadAllBytes(Stream content)
    {
        if (content == null)
        {
            return Array.Empty<byte>();
        }
        if (content is MemoryStream memoryStream)
        {
            return memoryStream.ToArray();
        }
        long position = (content.CanSeek ? content.Position : 0);
        using MemoryStream memoryStream2 = new MemoryStream();
        content.CopyTo(memoryStream2);
        if (content.CanSeek)
        {
            content.Position = position;
        }
        return memoryStream2.ToArray();
    }
}
