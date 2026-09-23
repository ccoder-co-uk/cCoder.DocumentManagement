// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.DocumentManagement.Brokers;
using cCoder.DocumentManagement.Exposures;
using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Foundations;
using cCoder.DocumentManagement.Services.Foundations.Events;

namespace cCoder.DocumentManagement.Services.Aggregations;

internal partial class FileMutationAggregationService(
    IFileService service,
    IFolderOperationsExposure folderOperationsExposure,
    IFileContentOperationsExposure fileContentOperationsExposure,
    IFileEventService eventService,
    IAuthorizationBroker authorizationBroker,
    IStreamBroker streamBroker)
    : IFileMutationAggregationService
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
            ValidateAllOnGet(inputs: [ignoreFilters]);
            return service.GetAll(ignoreFilters: ignoreFilters);

        });

    public ValueTask<cCoder.Data.Models.DMS.File> AddFileAsync(cCoder.Data.Models.DMS.File newFile)
=>
        TryCatch(operation: async () =>
        {
            ValidateFileOnAdd(inputs: [newFile]);
            Folder folder = folderOperationsExposure.GetFolderWithRoles(folderId: newFile.FolderId, ignoreFilters: true);


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

            string fileName = string.IsNullOrWhiteSpace(value: newFile.Name)
                ? GetPathName(path: relativePath)
                : newFile.Name;


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

                await fileContentOperationsExposure.AddOrUpdateFileContent(items: contents);
            }


            cCoder.Data.Models.DMS.File result =
                service.GetWithFolderAndContents(fileId: createdFile.Id, ignoreFilters: true);

            await eventService.RaiseFileAddEventAsync(entity: result);

            return result;

        });

    public cCoder.Data.Models.DMS.File GetByPath(int appId, string path)
=>
        TryCatch(operation: () =>
        {
            ValidateByPathOnGet(inputs: [appId, path]);
            cCoder.Data.Models.DMS.File byPath = service.GetByPath(appId: appId, path: path, ignoreFilters: true);


            if (byPath != null)
            {
                return byPath;
            }


            throw new SecurityException(message: "Access Denied!");

        });

    public ValueTask DeleteAsync(Guid fileId)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [fileId]);

            cCoder.Data.Models.DMS.File entity = service.GetAll(ignoreFilters: true)
                .FirstOrDefault(predicate: file => file.Id == fileId);


            if (entity == null)
            {
                return;
            }

            await eventService.RaiseFileDeleteEventAsync(entity: entity);
            await DeleteWithoutEventAsync(fileId: fileId);

        });

    public ValueTask<cCoder.Data.Models.DMS.File> UpdateFileAsync(cCoder.Data.Models.DMS.File updatedFile)
=>
        TryCatch(operation: async () =>
        {
            ValidateFileOnUpdate(inputs: (object[])[updatedFile]);
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
                dbVersion.Folder = ((updatedFile.FolderId == originalFolderId) ? dbVersion.Folder : folderOperationsExposure.GetFolderWithRoles(folderId: (Guid)updatedFile.FolderId, ignoreFilters: true));
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

                await fileContentOperationsExposure.AddOrUpdateFileContent(items: contents);
            }


            cCoder.Data.Models.DMS.File result =
                service.GetWithFolderAndContents(fileId: savedFile.Id, ignoreFilters: true);

            await eventService.RaiseFileUpdateEventAsync(entity: result);

            return result;

        });

    public ValueTask HandleFileDeleteEventAsync(cCoder.Data.Models.DMS.File file)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [file]);
            return fileContentOperationsExposure.DeleteAllForFileAsync(fileId: file.Id);

        });

    internal DMSResult GetAppPath(int appId, string path, int version) =>
        GetFileAppPath(
            appId: appId,
            path: path,
            version: version);

    private DMSResult GetFileAppPath(int appId, string path, int version)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [appId, path, version]);

            if (!IsFilePath(path: path))
            {
                throw new InvalidOperationException(message: "To get a folder archive, use folder processing operations.");
            }


            cCoder.Data.Models.DMS.File byPathWithFolderAndContents = service.GetByPathWithFolderAndContents(appId: appId, path: GetLoweredPath(path: path));


            if (byPathWithFolderAndContents == null)
            {
                throw new SecurityException(message: "Access Denied!");
            }


            return new DMSResult
            {
                MimeType = byPathWithFolderAndContents.MimeType,
                Data = streamBroker.Create(
                    content: GetContent(
                        file: byPathWithFolderAndContents,
                        version: version))
            };

        });

    internal IEnumerable<cCoder.Data.Models.DMS.File> SearchApp(int appId, string needle)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [appId, needle]);

            return service.Search(appId: appId, needle: streamBroker.EncodeUtf8(content: needle))
    .AsEnumerable();

        });

    internal ValueTask SaveAppPathAsync(int appId, string path, Stream content) =>
        SaveFileAppPathAsync(
            appId: appId,
            path: path,
            content: content);

    private ValueTask SaveFileAppPathAsync(int appId, string path, Stream content)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [appId, path, content]);

            if (IsFilePath(path: path))
            {
                cCoder.Data.Models.DMS.File existingFile = service.GetByPath(appId: appId, path: GetLoweredPath(path: path));
                byte[] rawBytes = ReadAllBytes(content: content);
                Folder folder = await BuildPathAppAsync(appId: appId, folderPath: GetParentPath(path: path));

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

    internal ValueTask DropAppPathAsync(int appId, string path, int version) =>
        DropFilePathAsync(
            appId: appId,
            path: path,
            version: version);

    private ValueTask DropFilePathAsync(int appId, string path, int version)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [appId, path, version]);

            if (IsFilePath(path: path))
            {
                await DropFileAppPathAsync(appId: appId, path: path, version: version);
                return;
            }


            throw new InvalidOperationException(message: "To delete a folder, use folder processing operations.");

        });

    internal ValueTask CopyAppPathAsync(int appId, string oldPath, string newPath) =>
        CopyFilePathAsync(
            appId: appId,
            oldPath: oldPath,
            newPath: newPath);

    private ValueTask CopyFilePathAsync(int appId, string oldPath, string newPath)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [appId, oldPath, newPath]);

            if (!IsFilePath(path: oldPath))
            {
                throw new InvalidOperationException(message: "To copy a folder, use folder processing operations.");
            }


            await CopyFileAppPathAsync(appId: appId, oldPath: oldPath, newPath: newPath);

        });

    internal ValueTask MoveAppPathAsync(int appId, string oldPath, string newPath) =>
        MoveFilePathAsync(
            appId: appId,
            oldPath: oldPath,
            newPath: newPath);

    private ValueTask MoveFilePathAsync(int appId, string oldPath, string newPath)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [appId, oldPath, newPath]);

            if (!IsFilePath(path: oldPath))
            {
                throw new InvalidOperationException(message: "To move a folder, use folder processing operations.");
            }


            string newParentPath = GetParentPath(path: newPath);
            string oldParentPath = GetParentPath(path: oldPath);

            Folder newParent = string.IsNullOrEmpty(value: newParentPath)
                ? null
                : folderOperationsExposure.GetFolderByPathWithRoles(appId: appId, path: GetLoweredPath(path: newParentPath));

            Folder oldParent = string.IsNullOrEmpty(value: oldParentPath)
                ? null
                : folderOperationsExposure.GetFolderByPathWithRoles(appId: appId, path: GetLoweredPath(path: oldParentPath));

            bool userIsAdmin = GetCurrentUser()
                .IsAdminOfApp(appId: appId);


            if (newParent == null && !IsFilePath(path: newPath))
            {
                newParent = await BuildPathAppAsync(appId: appId, folderPath: newPath);
            }


            if (newParent == null && IsFilePath(path: newPath))
            {
                newParent = await BuildPathAppAsync(appId: appId, folderPath: newParentPath);
            }


            await MoveFileAppPathFolderAsync(appId: appId, oldPath: oldPath, newPath: newPath, newParent: newParent, oldParent: oldParent, userIsAdmin: userIsAdmin);

        });

    public ValueTask<IEnumerable<Result<cCoder.Data.Models.DMS.File>>> AddOrUpdateFile(IEnumerable<cCoder.Data.Models.DMS.File> items)
=>
        TryCatch(operation: async () =>
        {
            ValidateOrUpdateFileOnAdd(inputs: [items]);
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
            ValidateAllFileOnDelete(inputs: [deletedFile]);

            foreach (cCoder.Data.Models.DMS.File item in deletedFile)
            {
                await DeleteWithoutEventAsync(fileId: item.Id);
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

    private async ValueTask BuildNewFileAppPathFolderAsync(int appId, string path, byte[] rawBytes, Folder folder)
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
        int version = (from fileContent in fileContentOperationsExposure.GetAll()
                       where fileContent.FileId == existingFile.Id
                       orderby fileContent.Version descending
                       select fileContent.Version).First() + 1;

        await fileContentOperationsExposure.AddFileContentAsync(newFileContent: new FileContent
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

    private async ValueTask BuildLocalFilePathFolderAsync(string path, byte[] rawBytes, Folder folder)
    {
        cCoder.Data.Models.DMS.File fileObject = await service.AddFileAsync(newFile: new cCoder.Data.Models.DMS.File
        {
            CreatedBy = GetCurrentUser()
                .Id,
            CreatedOn = DateTimeOffset.UtcNow,
            Name = GetPathName(path: path),
            Path = GetLoweredPath(path: path),
            FolderId = folder.Id,
            Folder = folder,
            MimeType = GetMimeType(path: path),
            Size = GetSizeOf(content: rawBytes)
        });

        await fileContentOperationsExposure.AddFileContentAsync(newFileContent: new FileContent
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

    private async ValueTask DropFileAppPathAsync(int appId, string path, int version)
    {
        cCoder.Data.Models.DMS.File file = service.GetByPathWithFolderRolesAndContents(appId: appId, path: GetLoweredPath(path: path), ignoreFilters: true);

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
        FileContent versionedContent = fileContentOperationsExposure.GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: (FileContent fileContent) => fileContent.FileId == file.Id && fileContent.Version == version);

        if (versionedContent == null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        await fileContentOperationsExposure.DeleteFileContentAsync(fileContentId: versionedContent.Id);

        if (!fileContentOperationsExposure.GetAll(ignoreFilters: true)
            .Any(predicate: (FileContent fileContent) => fileContent.FileId == file.Id))
        {
            await service.DeleteAsync(fileId: file.Id);
        }
    }

    private async ValueTask MoveFileAppPathFolderAsync(int appId, string oldPath, string newPath, Folder newParent, Folder oldParent, bool userIsAdmin)
    {
        ConfirmUserCanMoveFilePathFolder(oldPath: oldPath, newPath: newPath, newParent: newParent, oldParent: oldParent, userIsAdmin: userIsAdmin);
        cCoder.Data.Models.DMS.File sourceFile = service.GetByPathWithFolderRolesAndContents(appId: appId, path: GetLoweredPath(path: oldPath), ignoreFilters: true);

        if (sourceFile == null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        cCoder.Data.Models.DMS.File destinationFile = service.GetByPathWithFolderAndContents(appId: appId, path: GetLoweredPath(path: newPath), ignoreFilters: true);

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

            await fileContentOperationsExposure.AddOrUpdateFileContent(items: copiedContents);
            await DropAppPathAsync(appId: appId, path: oldPath, version: 0);
        }
        else if (!IsFilePath(path: newPath))
        {
            Folder newPathFolder = await BuildPathAppAsync(appId: appId, folderPath: newPath);
            await MoveFileAppPathFolderAsync(appId: appId, oldPath: oldPath, newPath: newPathFolder.Path + "/" + GetPathName(path: oldPath), newParent: newPathFolder, oldParent: oldParent, userIsAdmin: userIsAdmin);
        }
        else
        {
            sourceFile.FolderId = newParent.Id;
            sourceFile.Folder = newParent;
            sourceFile.Name = GetPathName(path: newPath);
            sourceFile.Path = (newParent.Path + "/" + GetPathName(path: newPath)).ToLower();
            await service.UpdateFileAsync(updatedFile: sourceFile);
        }
    }

    private async ValueTask CopyFileAppPathAsync(int appId, string oldPath, string newPath)
    {
        string newParentPath = GetParentPath(path: newPath);
        string oldParentPath = GetParentPath(path: oldPath);
        Folder newParent = string.IsNullOrEmpty(value: newParentPath) ? null : folderOperationsExposure.GetFolderByPathWithRoles(appId: appId, path: GetLoweredPath(path: newParentPath));
        Folder oldParent = string.IsNullOrEmpty(value: oldParentPath) ? null : folderOperationsExposure.GetFolderByPathWithRoles(appId: appId, path: GetLoweredPath(path: oldParentPath));

        bool userIsAdmin = GetCurrentUser()
            .IsAdminOfApp(appId: appId);

        if (newParent == null && !IsFilePath(path: newPath))
        {
            newParent = await BuildPathAppAsync(appId: appId, folderPath: newPath);
        }

        if (newParent == null && IsFilePath(path: newPath))
        {
            newParent = await BuildPathAppAsync(appId: appId, folderPath: newParentPath);
        }

        ConfirmUserCanMoveFilePathFolder(oldPath: oldPath, newPath: newPath, newParent: newParent, oldParent: oldParent, userIsAdmin: userIsAdmin);
        cCoder.Data.Models.DMS.File sourceFile = service.GetByPathWithFolderRolesAndContents(appId: appId, path: GetLoweredPath(path: oldPath), ignoreFilters: true);

        if (sourceFile == null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        if (!IsFilePath(path: newPath))
        {
            await CopyFileAppPathAsync(appId: appId, oldPath: oldPath, newPath: (await BuildPathAppAsync(appId: appId, folderPath: newPath)).Path + "/" + GetPathName(path: oldPath));
            return;
        }

        cCoder.Data.Models.DMS.File destinationFile = service.GetByPathWithFolderAndContents(appId: appId, path: GetLoweredPath(path: newPath), ignoreFilters: true);

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

            await fileContentOperationsExposure.AddOrUpdateFileContent(items: copiedContents);
            return;
        }

        cCoder.Data.Models.DMS.File copiedFile = await service.AddFileAsync(newFile: new cCoder.Data.Models.DMS.File
        {
            CreatedBy = sourceFile.CreatedBy,
            CreatedOn = sourceFile.CreatedOn,
            Name = GetPathName(path: newPath),
            Path = (newParent.Path + "/" + GetPathName(path: newPath)).ToLower(),
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

        await fileContentOperationsExposure.AddOrUpdateFileContent(items: copiedFileContents);
    }

    private void ConfirmUserCanMoveFilePathFolder(string oldPath, string newPath, Folder newParent, Folder oldParent, bool userIsAdmin)
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

    private async ValueTask<Folder> BuildPathAppAsync(int appId, string folderPath)
    {
        if (folderPath.Length <= 0)
        {
            return null;
        }

        Folder existingFolder = folderOperationsExposure.GetFolderByPathWithRoles(appId: appId, path: GetLoweredPath(path: folderPath));

        if (existingFolder == null)
        {
            existingFolder = await CreateFolderAppPathAsync(appId: appId, folderPath: folderPath);
        }

        return existingFolder;
    }

    private async ValueTask<Folder> CreateFolderAppPathAsync(int appId, string folderPath)
    {
        string parentPath = GetParentPath(path: folderPath);
        Folder folder = string.IsNullOrEmpty(value: parentPath) ? null : await BuildPathAppAsync(appId: appId, folderPath: parentPath);
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

        Folder folder2 = folderOperationsExposure.GetFolderByPath(appId: appId, path: GetLoweredPath(path: folderPath)) ?? new Folder
        {
            Id = Guid.Empty,
            AppId = appId,
            Name = GetPathName(path: folderPath),
            Parent = parentFolder,
            ParentId = parentFolder?.Id,
            Path = GetLoweredPath(path: folderPath),
            Roles = folderRoles
        };

        if (folder2.Id == Guid.Empty)
        {
            folder2 = await folderOperationsExposure.AddFolderAsync(newFolder: folder2);
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

    private byte[] ReadAllBytes(Stream content)
    {
        if (content == null)
        {
            return Array.Empty<byte>();
        }

        return streamBroker.ReadAllBytes(source: content);
    }

    private static byte[] GetContent(
        cCoder.Data.Models.DMS.File file,
        int version) =>
        version > 0
            ? file.Contents.FirstOrDefault(
                predicate: content => content.Version == version)?.RawData
            : file.Contents.OrderBy(
                    keySelector: content => content.Version)
                .Last().RawData;

    private ValueTask<cCoder.Data.Models.DMS.File> AddFileValueAsync(
        cCoder.Data.Models.DMS.File newFile) =>
        AddFileAsync(newFile: newFile);

    private ValueTask<cCoder.Data.Models.DMS.File> UpdateFileValueAsync(
        cCoder.Data.Models.DMS.File updatedFile) =>
        UpdateFileAsync(updatedFile: updatedFile);

    private ValueTask DeleteValueAsync(Guid fileId) =>
        DeleteAsync(fileId: fileId);

    private ValueTask DeleteWithoutEventAsync(Guid fileId) =>
        service.DeleteAsync(fileId: fileId);

    private static string NormalizePath(string path) =>
        (path ?? string.Empty).Trim()
            .TrimEnd(trimChar: '/');

    private static string GetLoweredPath(string path) =>
        NormalizePath(path: path)
            .ToLower();

    private static string GetPathName(string path) =>
        NormalizePath(path: path)
            .Split(separator: '/')
            .LastOrDefault();

    private static string GetParentPath(string path)
    {
        string normalizedPath = NormalizePath(path: path);
        string[] segments = normalizedPath.Split(separator: '/');

        return segments.Length > 1
            ? normalizedPath[..(normalizedPath.Length - (1 + segments.Last().Length))]
            : string.Empty;
    }

    private static bool IsFilePath(string path) =>
        GetPathExtension(path: path).Length > 0;

    private static string GetPathExtension(string path)
    {
        string name = GetPathName(path: path);

        return name?.Contains(value: '.') == true
            ? name
                .Split(separator: '.')
                .Last()
                .ToLower()
            : string.Empty;
    }

    private static string GetMimeType(string path) =>
        GetPathExtension(path: path) switch
        {
            "json" => "application/json",
            "pdf" => "application/pdf",
            "svg" => "image/svg+xml",
            "xml" => "application/xml",
            "zip" => "application/zip",
            _ => "text/plain"
        };
}