// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using System.Text;
using cCoder.DocumentManagement.Brokers;
using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Foundations;

namespace cCoder.DocumentManagement.Services.Processings;

internal partial class FileProcessingService(IFileService service, IFolderService folderService, IFileContentService fileContentService, IFileContentProcessingService fileContentProcessingService, IAuthorizationBroker authorizationBroker) : IFileProcessingService
{
    private User GetCurrentUser() =>
        authorizationBroker.GetCurrentUser();

    public cCoder.Data.Models.DMS.File Get(Guid fileId)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [fileId]);
            return service.Get(fileId: fileId);

        });

    public IQueryable<cCoder.Data.Models.DMS.File> GetAll(bool ignoreFilters = false)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [ignoreFilters]);
            return service.GetAll(ignoreFilters: ignoreFilters);

        });

    public ValueTask<cCoder.Data.Models.DMS.File> AddFileAsync(cCoder.Data.Models.DMS.File newFile)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [newFile]);
            Folder folder = folderService.GetWithRoles(folderId: newFile.FolderId, ignoreFilters: true);


            if (folder == null)
            {
                throw new SecurityException(message: "Access Denied!");
            }


            if (!GetCurrentUser()
                .IsAdminOfApp(appId: folder.AppId) && !folder.UserCan(user: GetCurrentUser(), privilege: "file_create"))
            {
                throw new SecurityException(message: "Access Denied!");
            }


            string relativePath = (string.IsNullOrWhiteSpace(value: newFile.Path) ? newFile.Name : newFile.Path);

            string fileName = (string.IsNullOrWhiteSpace(value: newFile.Name) ? new cCoder.DocumentManagement.Dependencies.Path(path: relativePath).Name : newFile.Name);


            cCoder.Data.Models.DMS.File createdFile = await service.AddFileAsync(newFile: new cCoder.Data.Models.DMS.File
            {
                FolderId = folder.Id,
                Folder = folder,
                Name = fileName,
                Description = newFile.Description,
                Path = (folder.Path + "/" + relativePath).Trim(trimChar: '/')
                .ToLowerInvariant(),
                MimeType = newFile.MimeType,
                Size = newFile.Size
            });


            if (newFile.Contents != null && newFile.Contents.Any())
            {
                FileContent[] contents = newFile.Contents.Select(selector: (FileContent content) => new FileContent
                {
                    Id = content.Id,
                    FileId = createdFile.Id,
                    File = createdFile,
                    Description = content.Description,
                    Size = content.Size,
                    Version = content.Version,
                    RawData = content.RawData
                })
                    .ToArray();

                await fileContentProcessingService.AddOrUpdateFileContent(items: contents);
            }


            return service.GetWithFolderAndContents(fileId: createdFile.Id, ignoreFilters: true);

        });

    public cCoder.Data.Models.DMS.File GetByPath(int appId, string path)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [appId, path]);
            cCoder.Data.Models.DMS.File byPath = service.GetByPath(appId: appId, path: path, ignoreFilters: true);


            if (byPath != null)
            {
                return byPath;
            }


            throw new SecurityException(message: "Access Denied!");

        });

    public ValueTask DeleteAsync(Guid fileId)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [fileId]);
            return service.DeleteAsync(fileId: fileId);

        });

    public ValueTask<cCoder.Data.Models.DMS.File> UpdateFileAsync(cCoder.Data.Models.DMS.File updatedFile)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: (object[])[updatedFile]);
            cCoder.Data.Models.DMS.File dbVersion = service.GetWithFolderRolesAndContents(fileId: (Guid)updatedFile.Id, ignoreFilters: true);


            if (dbVersion == null || !dbVersion.UserCan(user: GetCurrentUser(), privilege: "file_update"))
            {
                throw new SecurityException(message: "Access Denied!");
            }


            dbVersion.Description = updatedFile.Description;

            dbVersion.Size = updatedFile.Size;

            dbVersion.MimeType = updatedFile.MimeType;


            if (dbVersion.Name != updatedFile.Name || dbVersion.FolderId != updatedFile.FolderId)
            {
                Guid originalFolderId = dbVersion.FolderId;
                dbVersion.Name = updatedFile.Name;
                dbVersion.FolderId = updatedFile.FolderId;
                dbVersion.Folder = ((updatedFile.FolderId == originalFolderId) ? dbVersion.Folder : folderService.GetWithRoles(folderId: (Guid)updatedFile.FolderId, ignoreFilters: true));
                dbVersion.RecomputePath();
            }


            cCoder.Data.Models.DMS.File savedFile =
                await service.UpdateFileAsync(updatedFile: dbVersion);


            if (savedFile.Contents != null && savedFile.Contents.Any())
            {
                FileContent[] contents = savedFile.Contents.Select(selector: (FileContent content) => new FileContent
                {
                    Id = content.Id,
                    FileId = savedFile.Id,
                    File = savedFile,
                    Description = content.Description,
                    Size = content.Size,
                    CreatedBy = content.CreatedBy,
                    CreatedOn = content.CreatedOn,
                    Version = content.Version,
                    RawData = content.RawData
                })
                    .ToArray();

                await fileContentProcessingService.AddOrUpdateFileContent(items: contents);
            }


            return service.GetWithFolderAndContents(fileId: savedFile.Id, ignoreFilters: true);

        });

    public ValueTask HandleFileDeleteEventAsync(cCoder.Data.Models.DMS.File file)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [file]);
            return fileContentService.DeleteAllForFileAsync(fileId: file.Id);

        });

    public DMSResult GetAppPath(int appId, cCoder.DocumentManagement.Dependencies.Path path, int version = 0)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [appId, path, version]);

            if (!path.IsToFile)
            {
                throw new InvalidOperationException(message: "To get a folder archive, use folder processing operations.");
            }


            cCoder.Data.Models.DMS.File byPathWithFolderAndContents = service.GetByPathWithFolderAndContents(appId: appId, path: path.Lowered);


            if (byPathWithFolderAndContents == null)
            {
                throw new SecurityException(message: "Access Denied!");
            }


            return new DMSResult
            {
                MimeType = byPathWithFolderAndContents.MimeType,
                Data = byPathWithFolderAndContents.GetContent(version: version)
            };

        });

    public IEnumerable<cCoder.Data.Models.DMS.File> SearchApp(int appId, string needle)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [appId, needle]);

            return service.Search(appId: appId, needle: Encoding.UTF8.GetBytes(s: needle))
    .AsEnumerable();

        });

    public ValueTask SaveAppPathAsync(int appId, cCoder.DocumentManagement.Dependencies.Path path, Stream content = null)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [appId, path, content]);

            if (path.IsToFile)
            {
                cCoder.Data.Models.DMS.File existingFile = service.GetByPath(appId: appId, path: path.Lowered);
                byte[] rawBytes = ReadAllBytes(content: content);
                Folder folder = await BuildPathAppAsync(appId: appId, folderPath: path.ParentPath);

                if (existingFile == null)
                {
                    await BuildNewFileAppPathFolderAsync(appId: appId, path: path, rawBytes: rawBytes, folder: folder);
                }
                else
                {
                    await UpdateFileAppFolderAsync(
                        appId: appId,
                        updatedFile: existingFile,
                        rawBytes: rawBytes,
                        folder: folder);
                }
            }
            else
            {
                await BuildPathAppAsync(appId: appId, folderPath: path);
            }

        });

    public ValueTask DropAppPathAsync(int appId, cCoder.DocumentManagement.Dependencies.Path path, int version = 0)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [appId, path, version]);

            if (path.IsToFile)
            {
                await DropFileAppPathAsync(appId: appId, path: path, version: version);
                return;
            }


            throw new InvalidOperationException(message: "To delete a folder, use folder processing operations.");

        });

    public ValueTask CopyAppPathAsync(int appId, cCoder.DocumentManagement.Dependencies.Path oldPath, cCoder.DocumentManagement.Dependencies.Path newPath)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [appId, oldPath, newPath]);

            if (!oldPath.IsToFile)
            {
                throw new InvalidOperationException(message: "To copy a folder, use folder processing operations.");
            }


            await CopyFileAppPathAsync(appId: appId, oldPath: oldPath, newPath: newPath);

        });

    public ValueTask MoveAppPathAsync(int appId, cCoder.DocumentManagement.Dependencies.Path oldPath, cCoder.DocumentManagement.Dependencies.Path newPath)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [appId, oldPath, newPath]);

            if (!oldPath.IsToFile)
            {
                throw new InvalidOperationException(message: "To move a folder, use folder processing operations.");
            }


            Folder newParent = ((!string.IsNullOrEmpty(value: newPath.ParentPath.Lowered)) ? folderService.GetByPathWithRoles(appId: appId, path: newPath.ParentPath.Lowered) : null);

            Folder oldParent = ((!string.IsNullOrEmpty(value: oldPath.ParentPath.Lowered)) ? folderService.GetByPathWithRoles(appId: appId, path: oldPath.ParentPath.Lowered) : null);

            bool userIsAdmin = GetCurrentUser()
                .IsAdminOfApp(appId: appId);


            if (newParent == null && !newPath.IsToFile)
            {
                newParent = await BuildPathAppAsync(appId: appId, folderPath: newPath);
            }


            if (newParent == null && newPath.IsToFile)
            {
                newParent = await BuildPathAppAsync(appId: appId, folderPath: newPath.ParentPath);
            }


            await MoveFileAppPathFolderAsync(appId: appId, oldPath: oldPath, newPath: newPath, newParent: newParent, oldParent: oldParent, userIsAdmin: userIsAdmin);

        });

    public ValueTask<IEnumerable<Result<cCoder.Data.Models.DMS.File>>> AddOrUpdateFile(IEnumerable<cCoder.Data.Models.DMS.File> items)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [items]);
            List<Result<cCoder.Data.Models.DMS.File>> results = new List<Result<cCoder.Data.Models.DMS.File>>();


            foreach (cCoder.Data.Models.DMS.File item in items)
            {
                try
                {
                    cCoder.Data.Models.DMS.File savedItem = item.Id == Guid.Empty ? await AddFileValueAsync(newFile: item) : await UpdateFileValueAsync(updatedFile: item);

                    results.Add(item: new Result<cCoder.Data.Models.DMS.File>
                    {
                        Success = true,
                        Item = savedItem,
                        Message = item.Id == Guid.Empty ? "Added Successfully" : "Updated Successfully"
                    });
                }
                catch (Exception ex)
                {
                    results.Add(item: new Result<cCoder.Data.Models.DMS.File>
                    {
                        Success = false,
                        Item = item,
                        Message = ex.Message
                    });
                }
            }


            return (IEnumerable<Result<cCoder.Data.Models.DMS.File>>)results;

        });

    public ValueTask DeleteAllFileAsync(IEnumerable<cCoder.Data.Models.DMS.File> deletedFile)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [deletedFile]);

            foreach (cCoder.Data.Models.DMS.File item in deletedFile)
            {
                await DeleteValueAsync(fileId: item.Id);
            }

        });

    private async ValueTask UpdateFileAppFolderAsync(
        int appId,
        cCoder.Data.Models.DMS.File updatedFile,
        byte[] rawBytes,
        Folder folder)
    {
        if (!GetCurrentUser()
            .IsAdminOfApp(appId: appId) && !folder.UserCan(user: GetCurrentUser(), privilege: "file_update"))
        {
            throw new SecurityException(message: "Access Denied!");
        }

        await SaveFileVersionAsync(existingFile: updatedFile, rawBytes: rawBytes);
    }

    private async ValueTask BuildNewFileAppPathFolderAsync(int appId, cCoder.DocumentManagement.Dependencies.Path path, byte[] rawBytes, Folder folder)
    {
        if (!GetCurrentUser()
            .IsAdminOfApp(appId: appId) && !folder.UserCan(user: GetCurrentUser(), privilege: "file_create"))
        {
            throw new SecurityException(message: "Access Denied!");
        }

        await BuildLocalFilePathFolderAsync(path: path, rawBytes: rawBytes, folder: folder);
    }

    private async ValueTask SaveFileVersionAsync(cCoder.Data.Models.DMS.File existingFile, byte[] rawBytes)
    {
        int version = (from fileContent in fileContentProcessingService.GetAll()
                       where fileContent.FileId == existingFile.Id
                       orderby fileContent.Version descending
                       select fileContent.Version).First() + 1;

        await fileContentProcessingService.AddFileContentAsync(newFileContent: new FileContent
        {
            CreatedBy = GetCurrentUser()
                .Id,
            CreatedOn = DateTimeOffset.UtcNow,
            FileId = existingFile.Id,
            Version = version,
            Size = GetSizeOf(content: rawBytes),
            RawData = rawBytes,
            File = existingFile
        });
    }

    private async ValueTask BuildLocalFilePathFolderAsync(cCoder.DocumentManagement.Dependencies.Path path, byte[] rawBytes, Folder folder)
    {
        cCoder.Data.Models.DMS.File fileObject = await service.AddFileAsync(newFile: new cCoder.Data.Models.DMS.File
        {
            CreatedBy = GetCurrentUser()
                .Id,
            CreatedOn = DateTimeOffset.UtcNow,
            Name = path.Name,
            Path = path.Lowered,
            FolderId = folder.Id,
            Folder = folder,
            MimeType = path.MimeType,
            Size = GetSizeOf(content: rawBytes)
        });

        await fileContentProcessingService.AddFileContentAsync(newFileContent: new FileContent
        {
            CreatedBy = GetCurrentUser()
                .Id,
            CreatedOn = DateTimeOffset.UtcNow,
            FileId = fileObject.Id,
            File = fileObject,
            Version = 1,
            Size = GetSizeOf(content: rawBytes),
            RawData = rawBytes
        });
    }

    private async ValueTask DropFileAppPathAsync(int appId, cCoder.DocumentManagement.Dependencies.Path path, int version)
    {
        cCoder.Data.Models.DMS.File file = service.GetByPathWithFolderRolesAndContents(appId: appId, path: path.Lowered, ignoreFilters: true);

        if (file == null || !file.UserCan(user: GetCurrentUser(), privilege: "file_delete"))
        {
            throw new SecurityException(message: "Access Denied!");
        }

        if (version != 0)
        {
            await DropFileVersionAsync(version: version, file: file);
        }
        else
        {
            await service.DeleteAsync(fileId: file.Id);
        }
    }

    private async ValueTask DropFileVersionAsync(int version, cCoder.Data.Models.DMS.File file)
    {
        FileContent versionedContent = fileContentProcessingService.GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: (FileContent fileContent) => fileContent.FileId == file.Id && fileContent.Version == version);

        if (versionedContent == null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        await fileContentProcessingService.DeleteAsync(fileContentId: versionedContent.Id);

        if (!fileContentProcessingService.GetAll(ignoreFilters: true)
            .Any(predicate: (FileContent fileContent) => fileContent.FileId == file.Id))
        {
            await service.DeleteAsync(fileId: file.Id);
        }
    }

    private async ValueTask MoveFileAppPathFolderAsync(int appId, cCoder.DocumentManagement.Dependencies.Path oldPath, cCoder.DocumentManagement.Dependencies.Path newPath, Folder newParent, Folder oldParent, bool userIsAdmin)
    {
        ConfirmUserCanMoveFilePathFolder(oldPath: oldPath, newPath: newPath, newParent: newParent, oldParent: oldParent, userIsAdmin: userIsAdmin);
        cCoder.Data.Models.DMS.File sourceFile = service.GetByPathWithFolderRolesAndContents(appId: appId, path: oldPath.Lowered, ignoreFilters: true);

        if (sourceFile == null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        cCoder.Data.Models.DMS.File destinationFile = service.GetByPathWithFolderAndContents(appId: appId, path: newPath.Lowered, ignoreFilters: true);

        if (destinationFile != null)
        {
            int latestContentVersion = (destinationFile.Contents.Any() ? destinationFile.Contents.Max(selector: (FileContent fileContent) => fileContent.Version) : 0);

            FileContent[] copiedContents = sourceFile.Contents.Select(selector: (FileContent content) => new FileContent
            {
                FileId = destinationFile.Id,
                File = destinationFile,
                Description = content.Description,
                Size = content.Size,
                CreatedBy = content.CreatedBy,
                CreatedOn = content.CreatedOn,
                Version = content.Version + latestContentVersion,
                RawData = content.RawData
            })
                .ToArray();

            await fileContentProcessingService.AddOrUpdateFileContent(items: copiedContents);
            await DropAppPathAsync(appId: appId, path: oldPath);
        }
        else if (!newPath.IsToFile)
        {
            Folder newPathFolder = await BuildPathAppAsync(appId: appId, folderPath: newPath);
            await MoveFileAppPathFolderAsync(appId: appId, oldPath: oldPath, newPath: new cCoder.DocumentManagement.Dependencies.Path(path: newPathFolder.Path + "/" + oldPath.Name), newParent: newPathFolder, oldParent: oldParent, userIsAdmin: userIsAdmin);
        }
        else
        {
            sourceFile.FolderId = newParent.Id;
            sourceFile.Folder = newParent;
            sourceFile.Name = newPath.Name;
            sourceFile.Path = (newParent.Path + "/" + newPath.Name).ToLower();
            await service.UpdateFileAsync(updatedFile: sourceFile);
        }
    }

    private async ValueTask CopyFileAppPathAsync(int appId, cCoder.DocumentManagement.Dependencies.Path oldPath, cCoder.DocumentManagement.Dependencies.Path newPath)
    {
        Folder newParent = ((!string.IsNullOrEmpty(value: newPath.ParentPath.Lowered)) ? folderService.GetByPathWithRoles(appId: appId, path: newPath.ParentPath.Lowered) : null);
        Folder oldParent = ((!string.IsNullOrEmpty(value: oldPath.ParentPath.Lowered)) ? folderService.GetByPathWithRoles(appId: appId, path: oldPath.ParentPath.Lowered) : null);

        bool userIsAdmin = GetCurrentUser()
            .IsAdminOfApp(appId: appId);

        if (newParent == null && !newPath.IsToFile)
        {
            newParent = await BuildPathAppAsync(appId: appId, folderPath: newPath);
        }

        if (newParent == null && newPath.IsToFile)
        {
            newParent = await BuildPathAppAsync(appId: appId, folderPath: newPath.ParentPath);
        }

        ConfirmUserCanMoveFilePathFolder(oldPath: oldPath, newPath: newPath, newParent: newParent, oldParent: oldParent, userIsAdmin: userIsAdmin);
        cCoder.Data.Models.DMS.File sourceFile = service.GetByPathWithFolderRolesAndContents(appId: appId, path: oldPath.Lowered, ignoreFilters: true);

        if (sourceFile == null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        if (!newPath.IsToFile)
        {
            await CopyFileAppPathAsync(appId: appId, oldPath: oldPath, newPath: new cCoder.DocumentManagement.Dependencies.Path(path: (await BuildPathAppAsync(appId: appId, folderPath: newPath)).Path + "/" + oldPath.Name));
            return;
        }

        cCoder.Data.Models.DMS.File destinationFile = service.GetByPathWithFolderAndContents(appId: appId, path: newPath.Lowered, ignoreFilters: true);

        if (destinationFile != null)
        {
            int latestContentVersion = (destinationFile.Contents.Any() ? destinationFile.Contents.Max(selector: (FileContent fileContent) => fileContent.Version) : 0);

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

            await fileContentProcessingService.AddOrUpdateFileContent(items: copiedContents);
            return;
        }

        cCoder.Data.Models.DMS.File copiedFile = await service.AddFileAsync(newFile: new cCoder.Data.Models.DMS.File
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

        await fileContentProcessingService.AddOrUpdateFileContent(items: copiedFileContents);
    }

    private void ConfirmUserCanMoveFilePathFolder(cCoder.DocumentManagement.Dependencies.Path oldPath, cCoder.DocumentManagement.Dependencies.Path newPath, Folder newParent, Folder oldParent, bool userIsAdmin)
    {
        if (!userIsAdmin && !(oldParent?.UserCan(user: GetCurrentUser(), privilege: "file_update") ?? false))
        {
            throw new SecurityException(message: "Access Denied!");
        }

        if (!userIsAdmin && !(newParent?.UserCan(user: GetCurrentUser(), privilege: "file_update") ?? false))
        {
            throw new SecurityException(message: "Access Denied!");
        }
    }

    private async ValueTask<Folder> BuildPathAppAsync(int appId, cCoder.DocumentManagement.Dependencies.Path folderPath)
    {
        if (folderPath.Length <= 0)
        {
            return null;
        }

        Folder existingFolder = folderService.GetByPathWithRoles(appId: appId, path: folderPath.Lowered);

        if (existingFolder == null)
        {
            existingFolder = await CreateFolderAppPathAsync(appId: appId, folderPath: folderPath);
        }

        return existingFolder;
    }

    private async ValueTask<Folder> CreateFolderAppPathAsync(int appId, cCoder.DocumentManagement.Dependencies.Path folderPath)
    {
        Folder folder = ((folderPath.ParentPath.Depth <= 0) ? null : (await BuildPathAppAsync(appId: appId, folderPath: folderPath.ParentPath)));
        Folder parentFolder = folder;

        bool userCanCreateInApp = GetCurrentUser()
            .IsAdminOfApp(appId: appId) && GetCurrentUser()
            .Can(appId: appId, operation: "folder_create");

        bool userCanCreateFolderInParentFolder = parentFolder?.UserCan(user: GetCurrentUser(), privilege: "folder_create") ?? false;

        if (!userCanCreateInApp && !userCanCreateFolderInParentFolder)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        List<FolderRole> folderRoles = ((parentFolder != null) ? parentFolder.Roles.Select(selector: (FolderRole folderRole) => new FolderRole
        {
            RoleId = folderRole.RoleId
        })
            .ToList() : new List<FolderRole>());

        Folder folder2 = folderService.GetByPath(appId: appId, path: folderPath.Lowered) ?? new Folder
        {
            Id = Guid.Empty,
            AppId = appId,
            Name = folderPath.Name,
            Parent = parentFolder,
            ParentId = parentFolder?.Id,
            Path = folderPath.Lowered,
            Roles = folderRoles
        };

        if (folder2.Id == Guid.Empty)
        {
            folder2 = await folderService.AddFolderAsync(newFolder: folder2);
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
        content.CopyTo(destination: memoryStream2);

        if (content.CanSeek)
        {
            content.Position = position;
        }

        return memoryStream2.ToArray();
    }

    private ValueTask<cCoder.Data.Models.DMS.File> AddFileValueAsync(
        cCoder.Data.Models.DMS.File newFile) =>
        AddFileAsync(newFile: newFile);

    private ValueTask<cCoder.Data.Models.DMS.File> UpdateFileValueAsync(
        cCoder.Data.Models.DMS.File updatedFile) =>
        UpdateFileAsync(updatedFile: updatedFile);

    private ValueTask DeleteValueAsync(Guid fileId) =>
        DeleteAsync(fileId: fileId);
}