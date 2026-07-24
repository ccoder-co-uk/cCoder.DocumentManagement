// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using DmsPath = cCoder.DocumentManagement.Dependencies.Path;
using DmsResult = cCoder.DocumentManagement.Dependencies.DMSResult;

namespace cCoder.DocumentManagement.Exposures;

public interface IDmsInstanceOperationsExposure
{
    DmsResult GetDmsPath(
        DmsPath path,
        int version = 0);

    ValueTask SaveDmsPathAsync(
        DmsPath path,
        Stream content = null);

    ValueTask MoveDmsPathAsync(
        DmsPath oldPath,
        DmsPath newPath);

    ValueTask CopyDmsPathAsync(
        DmsPath oldPath,
        DmsPath newPath);

    ValueTask DropDmsPathAsync(
        DmsPath path,
        int version = 0);
}