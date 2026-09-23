// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Services.Aggregations;
using DmsPath = cCoder.DocumentManagement.Models.Path;

namespace cCoder.DocumentManagement.Exposures;

internal sealed class FilePathOperationsExposure(
    FileMutationAggregationService fileProcessingService)
    : IFilePathOperationsExposure
{
    public DMSResult GetAppPath(int appId, string path, int version = 0) =>
        fileProcessingService.GetAppPath(appId: appId, path: path, version: version);

    public IEnumerable<cCoder.Data.Models.DMS.File> SearchApp(int appId, string needle) =>
        fileProcessingService.SearchApp(appId: appId, needle: needle);

    public ValueTask SaveAppPathAsync(int appId, string path, Stream content = null) =>
        fileProcessingService.SaveAppPathAsync(appId: appId, path: path, content: content);

    public ValueTask DropAppPathAsync(int appId, string path, int version = 0) =>
        fileProcessingService.DropAppPathAsync(appId: appId, path: path, version: version);

    public ValueTask CopyAppPathAsync(int appId, string oldPath, string newPath) =>
        fileProcessingService.CopyAppPathAsync(appId: appId, oldPath: oldPath, newPath: newPath);

    public ValueTask MoveAppPathAsync(int appId, string oldPath, string newPath) =>
        fileProcessingService.MoveAppPathAsync(appId: appId, oldPath: oldPath, newPath: newPath);

    public ValueTask SaveFilePathAsync(
        int appId,
        DmsPath path,
        Stream content) =>
        fileProcessingService.SaveAppPathAsync(
            appId: appId,
            path: path.FullPath,
            content: content);

    public ValueTask CopyFilePathAsync(
        int appId,
        DmsPath oldPath,
        DmsPath newPath) =>
        fileProcessingService.CopyAppPathAsync(
            appId: appId,
            oldPath: oldPath.FullPath,
            newPath: newPath.FullPath);
}