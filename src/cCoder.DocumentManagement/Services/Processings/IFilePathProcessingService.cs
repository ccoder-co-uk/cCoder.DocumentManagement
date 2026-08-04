// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.DocumentManagement.Services.Processings;

internal interface IFilePathProcessingService
{
    DMSResult GetAppPath(int appId, cCoder.DocumentManagement.Dependencies.Path path, int version = 0);

    IEnumerable<cCoder.Data.Models.DMS.File> SearchApp(int appId, string needle);

    ValueTask SaveAppPathAsync(int appId, cCoder.DocumentManagement.Dependencies.Path path, Stream content = null);

    ValueTask DropAppPathAsync(int appId, cCoder.DocumentManagement.Dependencies.Path path, int version = 0);

    ValueTask CopyAppPathAsync(int appId, cCoder.DocumentManagement.Dependencies.Path oldPath, cCoder.DocumentManagement.Dependencies.Path newPath);

    ValueTask MoveAppPathAsync(int appId, cCoder.DocumentManagement.Dependencies.Path oldPath, cCoder.DocumentManagement.Dependencies.Path newPath);
}