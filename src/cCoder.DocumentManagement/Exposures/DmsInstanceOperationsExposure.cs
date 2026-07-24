// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Services.Foundations;
using DmsPath = cCoder.DocumentManagement.Dependencies.Path;
using DmsResult = cCoder.DocumentManagement.Dependencies.DMSResult;

namespace cCoder.DocumentManagement.Exposures;

internal sealed class DmsInstanceOperationsExposure(
    IDmsInstanceService dmsInstanceService)
    : IDmsInstanceOperationsExposure
{
    public DmsResult GetDmsPath(
        DmsPath path,
        int version = 0) =>
        dmsInstanceService.Get(
            path: path,
            version: version);

    public ValueTask SaveDmsPathAsync(
        DmsPath path,
        Stream content = null) =>
        dmsInstanceService.SaveAsync(
            path: path,
            content: content);

    public ValueTask MoveDmsPathAsync(
        DmsPath oldPath,
        DmsPath newPath) =>
        dmsInstanceService.MoveAsync(
            oldPath: oldPath,
            newPath: newPath);

    public ValueTask CopyDmsPathAsync(
        DmsPath oldPath,
        DmsPath newPath) =>
        dmsInstanceService.CopyAsync(
            oldPath: oldPath,
            newPath: newPath);

    public ValueTask DropDmsPathAsync(
        DmsPath path,
        int version = 0) =>
        dmsInstanceService.DropAsync(
            path: path,
            version: version);
}