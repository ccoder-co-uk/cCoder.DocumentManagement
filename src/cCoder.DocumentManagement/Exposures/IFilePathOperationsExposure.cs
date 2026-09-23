// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using DmsPath = cCoder.DocumentManagement.Models.Path;

namespace cCoder.DocumentManagement.Exposures;

public interface IFilePathOperationsExposure
{
    DMSResult GetAppPath(int appId, string path, int version = 0);
    IEnumerable<cCoder.Data.Models.DMS.File> SearchApp(int appId, string needle);
    ValueTask SaveAppPathAsync(int appId, string path, Stream content = null);
    ValueTask DropAppPathAsync(int appId, string path, int version = 0);
    ValueTask CopyAppPathAsync(int appId, string oldPath, string newPath);
    ValueTask MoveAppPathAsync(int appId, string oldPath, string newPath);

    ValueTask SaveFilePathAsync(
        int appId,
        DmsPath path,
        Stream content);

    ValueTask CopyFilePathAsync(
        int appId,
        DmsPath oldPath,
        DmsPath newPath);
}