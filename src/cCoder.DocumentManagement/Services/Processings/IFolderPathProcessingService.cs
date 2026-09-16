// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.DocumentManagement.Services.Processings;

internal interface IFolderPathProcessingService
{
    DMSResult GetFilesZippedAppPath(int appId, IEnumerable<string> paths);

    DMSResult GetAppPath(int appId, string path, string search = "");

    ValueTask UnpackAppPathAsync(
        int appId,
        string path,
        Stream content,
        bool ignoreArchiveRoot = false);

    ValueTask SaveAppPathAsync(int appId, string path);

    ValueTask DropAppPathAsync(int appId, string path);

    ValueTask CopyAppPathAsync(int appId, string oldPath, string newPath);

    ValueTask MoveAppPathAsync(int appId, string oldPath, string newPath);
}