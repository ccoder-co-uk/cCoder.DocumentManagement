// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Services.Aggregations;
using DmsPath = cCoder.DocumentManagement.Models.Path;
using DmsResult = cCoder.DocumentManagement.Models.DMSResult;

namespace cCoder.DocumentManagement.Exposures;

internal sealed class DmsInstanceOperationsExposure(
    IDmsAggregationService dmsAggregationService)
    : IDmsInstanceOperationsExposure
{
    public DmsResult Get(string path, int version = 0, string search = "") =>
        GetDmsPath(path: new DmsPath { FullPath = path }, version: version, search: search);

    public ValueTask UnpackAsync(string path, Stream content, bool ignoreArchiveRoot = false) =>
        UnpackDmsPathAsync(
            path: new DmsPath { FullPath = path },
            content: content,
            ignoreArchiveRoot: ignoreArchiveRoot);

    public ValueTask SaveAsync(string path, Stream content = null) =>
        SaveDmsPathAsync(path: new DmsPath { FullPath = path }, content: content);

    public ValueTask DropAsync(string path, int version = 0) =>
        DropDmsPathAsync(path: new DmsPath { FullPath = path }, version: version);

    public ValueTask CopyAsync(string oldPath, string newPath) =>
        CopyDmsPathAsync(
            oldPath: new DmsPath { FullPath = oldPath },
            newPath: new DmsPath { FullPath = newPath });

    public ValueTask MoveAsync(string oldPath, string newPath) =>
        MoveDmsPathAsync(
            oldPath: new DmsPath { FullPath = oldPath },
            newPath: new DmsPath { FullPath = newPath });

    public DmsResult GetDmsPath(
        DmsPath path,
        int version = 0,
        string search = "") =>
        dmsAggregationService.GetDmsOperation(
            dmsOperation: new DmsOperation
            {
                Path = path.FullPath,
                Version = version,
                Search = search
            }).Result;

    public DmsResult GetFilesZipped(IEnumerable<string> paths) =>
        dmsAggregationService.GetFilesZippedDmsOperation(
            dmsOperation: new DmsOperation { Paths = paths }).Result;

    public ValueTask UnpackDmsPathAsync(
        DmsPath path,
        Stream content,
        bool ignoreArchiveRoot = false) =>
        ExecuteUnpackAsync(path: path.FullPath, content: content, ignoreArchiveRoot: ignoreArchiveRoot);

    public ValueTask SaveDmsPathAsync(
        DmsPath path,
        Stream content = null) =>
        ExecuteSaveAsync(path: path.FullPath, content: content);

    public ValueTask MoveDmsPathAsync(
        DmsPath oldPath,
        DmsPath newPath) =>
        ExecuteMoveAsync(oldPath: oldPath.FullPath, newPath: newPath.FullPath);

    public ValueTask CopyDmsPathAsync(
        DmsPath oldPath,
        DmsPath newPath) =>
        ExecuteCopyAsync(oldPath: oldPath.FullPath, newPath: newPath.FullPath);

    public ValueTask DropDmsPathAsync(
        DmsPath path,
        int version = 0) =>
        ExecuteDropAsync(path: path.FullPath, version: version);

    private async ValueTask ExecuteSaveAsync(string path, Stream content) =>
        _ = await dmsAggregationService.SaveDmsOperationAsync(
            dmsOperation: new DmsOperation { Path = path, Content = content });

    private async ValueTask ExecuteUnpackAsync(string path, Stream content, bool ignoreArchiveRoot) =>
        _ = await dmsAggregationService.UnpackDmsOperationAsync(
            dmsOperation: new DmsOperation
            {
                Path = path,
                Content = content,
                IgnoreArchiveRoot = ignoreArchiveRoot
            });

    private async ValueTask ExecuteMoveAsync(string oldPath, string newPath) =>
        _ = await dmsAggregationService.MoveDmsOperationAsync(
            dmsOperation: new DmsOperation { Path = oldPath, NewPath = newPath });

    private async ValueTask ExecuteCopyAsync(string oldPath, string newPath) =>
        _ = await dmsAggregationService.CopyDmsOperationAsync(
            dmsOperation: new DmsOperation { Path = oldPath, NewPath = newPath });

    private async ValueTask ExecuteDropAsync(string path, int version) =>
        _ = await dmsAggregationService.DropDmsOperationAsync(
            dmsOperation: new DmsOperation { Path = path, Version = version });
}