// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Services.Processings;
using DataFile = cCoder.Data.Models.DMS.File;
using LocalApp = cCoder.Data.Models.CMS.App;
using LocalPath = cCoder.DocumentManagement.Dependencies.Path;


namespace cCoder.DocumentManagement.Services.Orchestrations;

internal partial class DmsOrchestrationService(
    ICurrentAppResolverProcessingService currentAppResolver,
    IFilePathProcessingService fileProcessingService,
    IFolderPathProcessingService folderProcessingService
) : IDmsOrchestrationService
{
    public DmsOperation GetFilesZippedDmsOperation(DmsOperation dmsOperation) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [dmsOperation]);
            LocalApp app = currentAppResolver.ResolveCurrentApp();

            dmsOperation.Result =
                folderProcessingService.GetFilesZippedAppPath(
                appId: app.Id,
                paths: dmsOperation.Paths.Select(
                    selector: path =>
                        new LocalPath(path: path)));

            return dmsOperation;

        });

    public DmsOperation GetDmsOperation(DmsOperation dmsOperation) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [dmsOperation]);
            LocalApp app = currentAppResolver.ResolveCurrentApp();
            LocalPath localPath = new(path: dmsOperation.Path);


            dmsOperation.Result = localPath.IsToFile
                ? fileProcessingService.GetAppPath(appId: app.Id, path: localPath, version: dmsOperation.Version)
                : folderProcessingService.GetAppPath(appId: app.Id, path: localPath, search: dmsOperation.Search);

            return dmsOperation;

        });

    public DmsOperation SearchFilesDmsOperation(DmsOperation dmsOperation) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [dmsOperation]);
            LocalApp app = currentAppResolver.ResolveCurrentApp();

            dmsOperation.Files =
                fileProcessingService.SearchApp(
                    appId: app.Id,
                    needle: dmsOperation.Needle)
                .Select(selector: ToExternalFile)
                .ToArray();

            return dmsOperation;
        });

    public ValueTask<DmsOperation> UnpackDmsOperationAsync(DmsOperation dmsOperation) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [dmsOperation]);
            LocalApp app = currentAppResolver.ResolveCurrentApp();

            await folderProcessingService.UnpackAppPathAsync(
                appId: app.Id,
                path: new LocalPath(path: dmsOperation.Path),
                content: dmsOperation.Content,
                ignoreArchiveRoot: dmsOperation.IgnoreArchiveRoot);

            return dmsOperation;

        });

    public ValueTask<DmsOperation> SaveDmsOperationAsync(DmsOperation dmsOperation) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [dmsOperation]);
            LocalApp app = currentAppResolver.ResolveCurrentApp();
            LocalPath localPath = new(path: dmsOperation.Path);


            if (localPath.IsToFile)
            {
                await fileProcessingService.SaveAppPathAsync(
                    appId: app.Id,
                    path: localPath,
                    content: dmsOperation.Content);
            }
            else
            {
                await folderProcessingService.SaveAppPathAsync(appId: app.Id, path: localPath);
            }

            return dmsOperation;
        });

    public ValueTask<DmsOperation> DropDmsOperationAsync(DmsOperation dmsOperation) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [dmsOperation]);
            LocalApp app = currentAppResolver.ResolveCurrentApp();
            LocalPath localPath = new(path: dmsOperation.Path);


            if (localPath.IsToFile)
            {
                await fileProcessingService.DropAppPathAsync(
                    appId: app.Id,
                    path: localPath,
                    version: dmsOperation.Version);
            }
            else
            {
                await folderProcessingService.DropAppPathAsync(appId: app.Id, path: localPath);
            }

            return dmsOperation;
        });

    public ValueTask<DmsOperation> CopyDmsOperationAsync(DmsOperation dmsOperation) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [dmsOperation]);
            LocalApp app = currentAppResolver.ResolveCurrentApp();
            LocalPath sourcePath = new(path: dmsOperation.Path);
            LocalPath destinationPath = new(path: dmsOperation.NewPath);


            if (sourcePath.IsToFile)
            {
                await fileProcessingService.CopyAppPathAsync(appId: app.Id, oldPath: sourcePath, newPath: destinationPath);
            }
            else
            {
                await folderProcessingService.CopyAppPathAsync(appId: app.Id, oldPath: sourcePath, newPath: destinationPath);
            }

            return dmsOperation;
        });

    public ValueTask<DmsOperation> MoveDmsOperationAsync(DmsOperation dmsOperation) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [dmsOperation]);
            LocalApp app = currentAppResolver.ResolveCurrentApp();
            LocalPath sourcePath = new(path: dmsOperation.Path);
            LocalPath destinationPath = new(path: dmsOperation.NewPath);


            if (sourcePath.IsToFile)
            {
                await fileProcessingService.MoveAppPathAsync(appId: app.Id, oldPath: sourcePath, newPath: destinationPath);
            }
            else
            {
                await folderProcessingService.MoveAppPathAsync(appId: app.Id, oldPath: sourcePath, newPath: destinationPath);
            }

            return dmsOperation;
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