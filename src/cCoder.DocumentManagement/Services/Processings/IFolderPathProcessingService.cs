// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.DocumentManagement.Services.Processings;

internal interface IFolderPathProcessingService
{
    DMSResult GetFilesZippedAppPath(int appId, IEnumerable<cCoder.DocumentManagement.Models.Path> paths);

    DMSResult GetAppPath(int appId, cCoder.DocumentManagement.Models.Path path, string search = "");

    ValueTask UnpackAppPathAsync(
        int appId,
        cCoder.DocumentManagement.Models.Path path,
        Stream content,
        bool ignoreArchiveRoot = false);

    ValueTask SaveAppPathAsync(int appId, cCoder.DocumentManagement.Models.Path path);

    ValueTask DropAppPathAsync(int appId, cCoder.DocumentManagement.Models.Path path);

    ValueTask CopyAppPathAsync(int appId, cCoder.DocumentManagement.Models.Path oldPath, cCoder.DocumentManagement.Models.Path newPath);

    ValueTask MoveAppPathAsync(int appId, cCoder.DocumentManagement.Models.Path oldPath, cCoder.DocumentManagement.Models.Path newPath);
}