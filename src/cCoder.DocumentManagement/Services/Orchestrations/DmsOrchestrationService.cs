// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Services.Processings;
using DataFile = cCoder.Data.Models.DMS.File;
using DmsResult = cCoder.DocumentManagement.Dependencies.DMSResult;
using LocalApp = cCoder.Data.Models.CMS.App;
using LocalFile = cCoder.Data.Models.DMS.File;
using LocalPath = cCoder.DocumentManagement.Dependencies.Path;


namespace cCoder.DocumentManagement.Services.Orchestrations;

internal partial class DmsOrchestrationService(
    ICurrentAppResolverProcessingService currentAppResolver,
    IFileProcessingService fileProcessingService,
    IFolderProcessingService folderProcessingService
) : IDmsOrchestrationService
{
    public DmsResult GetFilesZipped(IEnumerable<LocalPath> paths)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [paths]);
            LocalApp app = currentAppResolver.ResolveCurrentApp();

            return folderProcessingService.GetFilesZippedAppPath(app: app, paths: paths);

        });

    public DmsResult Get(LocalPath path, int version = 0, string search = "")
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [path, version, search]);
            LocalApp app = currentAppResolver.ResolveCurrentApp();


            return path.IsToFile
                ? fileProcessingService.GetAppPath(app: app, path: path, version: version)
                : folderProcessingService.GetAppPath(app: app, path: path, search: search);

        });

    public IEnumerable<DataFile> Search(string needle)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [needle]);
            LocalApp app = currentAppResolver.ResolveCurrentApp();


            return fileProcessingService.SearchApp(app: app, needle: needle)
                .Select(selector: ToExternalFile)
                .ToArray();

        });

    public ValueTask UnpackAsync(LocalPath path, Stream content, bool ignoreArchiveRoot = false)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [path, content, ignoreArchiveRoot]);
            LocalApp app = currentAppResolver.ResolveCurrentApp();

            await folderProcessingService.UnpackAppPathAsync(app: app, path: path, content: content, ignoreArchiveRoot: ignoreArchiveRoot);

        });

    public ValueTask SaveAsync(LocalPath path, Stream content = null)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [path, content]);
            LocalApp app = currentAppResolver.ResolveCurrentApp();


            if (path.IsToFile)
            {
                await fileProcessingService.SaveAppPathAsync(app: app, path: path, content: content);
            }
            else
            {
                await folderProcessingService.SaveAppPathAsync(app: app, path: path);
            }

        });

    public ValueTask DropAsync(LocalPath path, int version = 0)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [path, version]);
            LocalApp app = currentAppResolver.ResolveCurrentApp();


            if (path.IsToFile)
            {
                await fileProcessingService.DropAppPathAsync(app: app, path: path, version: version);
            }
            else
            {
                await folderProcessingService.DropAppPathAsync(app: app, path: path);
            }

        });

    public ValueTask CopyAsync(LocalPath oldPath, LocalPath newPath)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [oldPath, newPath]);
            LocalApp app = currentAppResolver.ResolveCurrentApp();


            if (oldPath.IsToFile)
            {
                await fileProcessingService.CopyAppPathAsync(app: app, oldPath: oldPath, newPath: newPath);
            }
            else
            {
                await folderProcessingService.CopyAppPathAsync(app: app, oldPath: oldPath, newPath: newPath);
            }

        });

    public ValueTask MoveAsync(LocalPath oldPath, LocalPath newPath)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [oldPath, newPath]);
            LocalApp app = currentAppResolver.ResolveCurrentApp();


            if (oldPath.IsToFile)
            {
                await fileProcessingService.MoveAppPathAsync(app: app, oldPath: oldPath, newPath: newPath);
            }
            else
            {
                await folderProcessingService.MoveAppPathAsync(app: app, oldPath: oldPath, newPath: newPath);
            }

        });

    private static DataFile ToExternalFile(LocalFile file) =>
        file == null ? null : new DataFile
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
}