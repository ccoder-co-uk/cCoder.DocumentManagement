// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Services.Processings;
using DataFile = cCoder.Data.Models.DMS.File;
using LocalApp = cCoder.Data.Models.CMS.App;
using LocalPath = cCoder.DocumentManagement.Models.Path;


namespace cCoder.DocumentManagement.Services.Orchestrations;

internal partial class DmsOrchestrationService(
    ICurrentAppResolverProcessingService currentAppResolver,
    IFilePathProcessingService fileProcessingService,
    IFolderPathProcessingService folderProcessingService
) : IDmsOrchestrationService
{
    public DmsOperation GetFilesZippedDmsOperation(DmsOperation operation) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [operation]);
            LocalApp app = currentAppResolver.ResolveCurrentApp();

            operation.Result =
                folderProcessingService.GetFilesZippedAppPath(
                appId: app.Id,
                paths: operation.Paths.Select(
                    selector: path =>
                        new LocalPath(path: path)));

            return operation;

        });

    public DmsOperation GetDmsOperation(DmsOperation operation) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [operation]);
            LocalApp app = currentAppResolver.ResolveCurrentApp();
            LocalPath localPath = new(path: operation.Path);


            operation.Result = localPath.IsToFile
                ? fileProcessingService.GetAppPath(appId: app.Id, path: localPath, version: operation.Version)
                : folderProcessingService.GetAppPath(appId: app.Id, path: localPath, search: operation.Search);

            return operation;

        });

    public DmsOperation SearchFilesDmsOperation(DmsOperation operation) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [operation]);
            LocalApp app = currentAppResolver.ResolveCurrentApp();

            operation.Files =
                fileProcessingService.SearchApp(
                    appId: app.Id,
                    needle: operation.Needle)
                .Select(selector: ToExternalFile)
                .ToArray();

            return operation;
        });

    public ValueTask<DmsOperation> UnpackDmsOperationAsync(DmsOperation operation) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [operation]);
            LocalApp app = currentAppResolver.ResolveCurrentApp();

            await folderProcessingService.UnpackAppPathAsync(
                appId: app.Id,
                path: new LocalPath(path: operation.Path),
                content: operation.Content,
                ignoreArchiveRoot: operation.IgnoreArchiveRoot);

            return operation;

        });

    public ValueTask<DmsOperation> SaveDmsOperationAsync(DmsOperation operation) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [operation]);
            LocalApp app = currentAppResolver.ResolveCurrentApp();
            LocalPath localPath = new(path: operation.Path);


            if (localPath.IsToFile)
            {
                await fileProcessingService.SaveAppPathAsync(
                    appId: app.Id,
                    path: localPath,
                    content: operation.Content);
            }
            else
            {
                await folderProcessingService.SaveAppPathAsync(appId: app.Id, path: localPath);
            }

            return operation;
        });

    public ValueTask<DmsOperation> DropDmsOperationAsync(DmsOperation operation) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [operation]);
            LocalApp app = currentAppResolver.ResolveCurrentApp();
            LocalPath localPath = new(path: operation.Path);


            if (localPath.IsToFile)
            {
                await fileProcessingService.DropAppPathAsync(
                    appId: app.Id,
                    path: localPath,
                    version: operation.Version);
            }
            else
            {
                await folderProcessingService.DropAppPathAsync(appId: app.Id, path: localPath);
            }

            return operation;
        });

    public ValueTask<DmsOperation> CopyDmsOperationAsync(DmsOperation operation) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [operation]);
            LocalApp app = currentAppResolver.ResolveCurrentApp();
            LocalPath sourcePath = new(path: operation.Path);
            LocalPath destinationPath = new(path: operation.NewPath);


            if (sourcePath.IsToFile)
            {
                await fileProcessingService.CopyAppPathAsync(appId: app.Id, oldPath: sourcePath, newPath: destinationPath);
            }
            else
            {
                await folderProcessingService.CopyAppPathAsync(appId: app.Id, oldPath: sourcePath, newPath: destinationPath);
            }

            return operation;
        });

    public ValueTask<DmsOperation> MoveDmsOperationAsync(DmsOperation operation) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [operation]);
            LocalApp app = currentAppResolver.ResolveCurrentApp();
            LocalPath sourcePath = new(path: operation.Path);
            LocalPath destinationPath = new(path: operation.NewPath);


            if (sourcePath.IsToFile)
            {
                await fileProcessingService.MoveAppPathAsync(appId: app.Id, oldPath: sourcePath, newPath: destinationPath);
            }
            else
            {
                await folderProcessingService.MoveAppPathAsync(appId: app.Id, oldPath: sourcePath, newPath: destinationPath);
            }

            return operation;
        });

    private static DataFile ToExternalFile(DataFile file) =>
        file is null
            ? null
            : new DataFile
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
                DeletedOn = file.DeletedOn
            };
}