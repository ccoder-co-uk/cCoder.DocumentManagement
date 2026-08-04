// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.DocumentManagement.Services.Processings;

internal interface IFolderPathProcessingService
{
    DMSResult GetFilesZippedAppPath(int appId, IEnumerable<cCoder.DocumentManagement.Dependencies.Path> paths);

    DMSResult GetAppPath(int appId, cCoder.DocumentManagement.Dependencies.Path path, string search = "");

    ValueTask UnpackAppPathAsync(
        int appId,
        cCoder.DocumentManagement.Dependencies.Path path,
        Stream content,
        bool ignoreArchiveRoot = false);

    ValueTask SaveAppPathAsync(int appId, cCoder.DocumentManagement.Dependencies.Path path);

    ValueTask DropAppPathAsync(int appId, cCoder.DocumentManagement.Dependencies.Path path);

    ValueTask CopyAppPathAsync(int appId, cCoder.DocumentManagement.Dependencies.Path oldPath, cCoder.DocumentManagement.Dependencies.Path newPath);

    ValueTask MoveAppPathAsync(int appId, cCoder.DocumentManagement.Dependencies.Path oldPath, cCoder.DocumentManagement.Dependencies.Path newPath);
}