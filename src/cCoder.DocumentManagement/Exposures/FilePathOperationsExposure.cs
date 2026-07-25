// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Services.Processings;
using DmsPath = cCoder.DocumentManagement.Dependencies.Path;

namespace cCoder.DocumentManagement.Exposures;

internal sealed class FilePathOperationsExposure(
    IFileProcessingService fileProcessingService)
    : IFilePathOperationsExposure
{
    public ValueTask SaveFilePathAsync(
        int appId,
        DmsPath path,
        Stream content) =>
        fileProcessingService.SaveAppPathAsync(
            appId: appId,
            path: path,
            content: content);

    public ValueTask CopyFilePathAsync(
        int appId,
        DmsPath oldPath,
        DmsPath newPath) =>
        fileProcessingService.CopyAppPathAsync(
            appId: appId,
            oldPath: oldPath,
            newPath: newPath);
}