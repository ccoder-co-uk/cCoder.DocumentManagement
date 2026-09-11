// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Services.Orchestrations;
using DmsFile = cCoder.Data.Models.DMS.File;
using DmsPath = cCoder.DocumentManagement.Dependencies.Path;
using DmsResult = cCoder.DocumentManagement.Models.DMSResult;


namespace cCoder.DocumentManagement.Exposures;

internal sealed class Dms(
    IDmsOrchestrationService dmsOrchestrationService)
    : IDms
{
    public DmsResult GetFilesZipped(IEnumerable<DmsPath> paths) =>
        dmsOrchestrationService.GetFilesZippedDmsOperation(
            dmsOperation: new DmsOperation
            {
                Paths = paths.Select(
                    selector: path =>
                        path.FullPath)
            })
            .Result;

    public DmsResult Get(DmsPath path, int version = 0, string search = "") =>
        dmsOrchestrationService.GetDmsOperation(
            dmsOperation: new DmsOperation
            {
                Path = path.FullPath,
                Version = version,
                Search = search
            })
            .Result;

    public IEnumerable<DmsFile> Search(string needle) =>
        dmsOrchestrationService.SearchFilesDmsOperation(
            dmsOperation: new DmsOperation
            {
                Needle = needle
            })
            .Files;

    public ValueTask UnpackAsync(DmsPath path, Stream content, bool ignoreArchiveRoot = false) =>
        ExecuteUnpackDmsOperationAsync(
            path: path,
            content: content,
            ignoreArchiveRoot: ignoreArchiveRoot);

    public ValueTask SaveAsync(DmsPath path, Stream content = null) =>
        ExecuteSaveDmsOperationAsync(path: path, content: content);

    public ValueTask DropAsync(DmsPath path, int version = 0) =>
        ExecuteDropDmsOperationAsync(path: path, version: version);

    public ValueTask CopyAsync(DmsPath oldPath, DmsPath newPath) =>
        ExecuteCopyDmsOperationAsync(oldPath: oldPath, newPath: newPath);

    public ValueTask MoveAsync(DmsPath oldPath, DmsPath newPath) =>
        ExecuteMoveDmsOperationAsync(oldPath: oldPath, newPath: newPath);

    private async ValueTask ExecuteUnpackDmsOperationAsync(
        DmsPath path,
        Stream content,
        bool ignoreArchiveRoot) =>
        _ = await dmsOrchestrationService.UnpackDmsOperationAsync(
            dmsOperation: new DmsOperation
            {
                Path = path.FullPath,
                Content = content,
                IgnoreArchiveRoot = ignoreArchiveRoot
            });

    private async ValueTask ExecuteSaveDmsOperationAsync(
        DmsPath path,
        Stream content) =>
        _ = await dmsOrchestrationService.SaveDmsOperationAsync(
            dmsOperation: new DmsOperation
            {
                Path = path.FullPath,
                Content = content
            });

    private async ValueTask ExecuteDropDmsOperationAsync(
        DmsPath path,
        int version) =>
        _ = await dmsOrchestrationService.DropDmsOperationAsync(
            dmsOperation: new DmsOperation
            {
                Path = path.FullPath,
                Version = version
            });

    private async ValueTask ExecuteCopyDmsOperationAsync(
        DmsPath oldPath,
        DmsPath newPath) =>
        _ = await dmsOrchestrationService.CopyDmsOperationAsync(
            dmsOperation: new DmsOperation
            {
                Path = oldPath.FullPath,
                NewPath = newPath.FullPath
            });

    private async ValueTask ExecuteMoveDmsOperationAsync(
        DmsPath oldPath,
        DmsPath newPath) =>
        _ = await dmsOrchestrationService.MoveDmsOperationAsync(
            dmsOperation: new DmsOperation
            {
                Path = oldPath.FullPath,
                NewPath = newPath.FullPath
            });
}