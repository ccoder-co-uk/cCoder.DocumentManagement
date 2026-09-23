// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Services.Aggregations;
using DmsFile = cCoder.Data.Models.DMS.File;
using DmsPath = cCoder.DocumentManagement.Models.Path;
using DmsResult = cCoder.DocumentManagement.Models.DMSResult;


namespace cCoder.DocumentManagement.Exposures;

internal sealed class Dms(
    IDmsAggregationService dmsAggregationService)
    : IDms
{
    public DmsResult GetFilesZipped(IEnumerable<DmsPath> paths) =>
        GetFilesZipped(paths: paths.Select(selector: path => path.FullPath));

    public DmsResult GetFilesZipped(IEnumerable<string> paths) =>
        dmsAggregationService.GetFilesZippedDmsOperation(
            dmsOperation: new DmsOperation
            {
                Paths = paths
            })
            .Result;

    public DmsResult Get(DmsPath path, int version = 0, string search = "") =>
        Get(path: path.FullPath, version: version, search: search);

    public DmsResult Get(string path, int version = 0, string search = "") =>
        dmsAggregationService.GetDmsOperation(
            dmsOperation: new DmsOperation
            {
                Path = path,
                Version = version,
                Search = search
            })
            .Result;

    public IEnumerable<DmsFile> Search(string needle) =>
        dmsAggregationService.SearchFilesDmsOperation(
            dmsOperation: new DmsOperation
            {
                Needle = needle
            })
            .Files;

    public ValueTask UnpackAsync(DmsPath path, Stream content, bool ignoreArchiveRoot = false) =>
        UnpackAsync(path: path.FullPath, content: content, ignoreArchiveRoot: ignoreArchiveRoot);

    public ValueTask UnpackAsync(string path, Stream content, bool ignoreArchiveRoot = false) =>
        ExecuteUnpackDmsOperationAsync(
            path: path,
            content: content,
            ignoreArchiveRoot: ignoreArchiveRoot);

    public ValueTask SaveAsync(DmsPath path, Stream content = null) =>
        SaveAsync(path: path.FullPath, content: content);

    public ValueTask SaveAsync(string path, Stream content = null) =>
        ExecuteSaveDmsOperationAsync(path: path, content: content);

    public ValueTask DropAsync(DmsPath path, int version = 0) =>
        DropAsync(path: path.FullPath, version: version);

    public ValueTask DropAsync(string path, int version = 0) =>
        ExecuteDropDmsOperationAsync(path: path, version: version);

    public ValueTask CopyAsync(DmsPath oldPath, DmsPath newPath) =>
        CopyAsync(oldPath: oldPath.FullPath, newPath: newPath.FullPath);

    public ValueTask CopyAsync(string oldPath, string newPath) =>
        ExecuteCopyDmsOperationAsync(oldPath: oldPath, newPath: newPath);

    public ValueTask MoveAsync(DmsPath oldPath, DmsPath newPath) =>
        MoveAsync(oldPath: oldPath.FullPath, newPath: newPath.FullPath);

    public ValueTask MoveAsync(string oldPath, string newPath) =>
        ExecuteMoveDmsOperationAsync(oldPath: oldPath, newPath: newPath);

    private async ValueTask ExecuteUnpackDmsOperationAsync(
        string path,
        Stream content,
        bool ignoreArchiveRoot) =>
        _ = await dmsAggregationService.UnpackDmsOperationAsync(
            dmsOperation: new DmsOperation
            {
                Path = path,
                Content = content,
                IgnoreArchiveRoot = ignoreArchiveRoot
            });

    private async ValueTask ExecuteSaveDmsOperationAsync(
        string path,
        Stream content) =>
        _ = await dmsAggregationService.SaveDmsOperationAsync(
            dmsOperation: new DmsOperation
            {
                Path = path,
                Content = content
            });

    private async ValueTask ExecuteDropDmsOperationAsync(
        string path,
        int version) =>
        _ = await dmsAggregationService.DropDmsOperationAsync(
            dmsOperation: new DmsOperation
            {
                Path = path,
                Version = version
            });

    private async ValueTask ExecuteCopyDmsOperationAsync(
        string oldPath,
        string newPath) =>
        _ = await dmsAggregationService.CopyDmsOperationAsync(
            dmsOperation: new DmsOperation
            {
                Path = oldPath,
                NewPath = newPath
            });

    private async ValueTask ExecuteMoveDmsOperationAsync(
        string oldPath,
        string newPath) =>
        _ = await dmsAggregationService.MoveDmsOperationAsync(
            dmsOperation: new DmsOperation
            {
                Path = oldPath,
                NewPath = newPath
            });
}