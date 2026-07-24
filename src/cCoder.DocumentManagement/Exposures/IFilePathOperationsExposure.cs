// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using DmsPath = cCoder.DocumentManagement.Dependencies.Path;

namespace cCoder.DocumentManagement.Exposures;

public interface IFilePathOperationsExposure
{
    ValueTask SaveFilePathAsync(
        int appId,
        DmsPath path,
        Stream content);

    ValueTask CopyFilePathAsync(
        int appId,
        DmsPath oldPath,
        DmsPath newPath);
}