// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.DocumentManagement.Services.Processings;

internal interface IFilePathProcessingService
{
    DMSResult GetAppPath(int appId, string path, int version = 0);

    IEnumerable<cCoder.Data.Models.DMS.File> SearchApp(int appId, string needle);

    ValueTask SaveAppPathAsync(int appId, string path, Stream content = null);

    ValueTask DropAppPathAsync(int appId, string path, int version = 0);

    ValueTask CopyAppPathAsync(int appId, string oldPath, string newPath);

    ValueTask MoveAppPathAsync(int appId, string oldPath, string newPath);
}