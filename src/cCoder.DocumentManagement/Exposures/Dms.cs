// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Services.Orchestrations;
using DmsFile = cCoder.Data.Models.DMS.File;
using DmsPath = cCoder.DocumentManagement.Models.Path;
using DmsResult = cCoder.DocumentManagement.Models.DMSResult;


namespace cCoder.DocumentManagement.Exposures;

public class Dms(IDmsOrchestrationService dmsOrchestrationService) : IDms
{
    public DmsResult GetFilesZipped(IEnumerable<DmsPath> paths) =>
        dmsOrchestrationService.GetFilesZipped(paths: paths);

    public DmsResult Get(DmsPath path, int version = 0, string search = "") =>
        dmsOrchestrationService.Get(path: path, version: version, search: search);

    public IEnumerable<DmsFile> Search(string needle) =>
        dmsOrchestrationService.Search(needle: needle);

    public ValueTask UnpackAsync(DmsPath path, Stream content, bool ignoreArchiveRoot = false) =>
        dmsOrchestrationService.UnpackAsync(path: path, content: content, ignoreArchiveRoot: ignoreArchiveRoot);

    public ValueTask SaveAsync(DmsPath path, Stream content = null) =>
        dmsOrchestrationService.SaveAsync(path: path, content: content);

    public ValueTask DropAsync(DmsPath path, int version = 0) =>
        dmsOrchestrationService.DropAsync(path: path, version: version);

    public ValueTask CopyAsync(DmsPath oldPath, DmsPath newPath) =>
        dmsOrchestrationService.CopyAsync(oldPath: oldPath, newPath: newPath);

    public ValueTask MoveAsync(DmsPath oldPath, DmsPath newPath) =>
        dmsOrchestrationService.MoveAsync(oldPath: oldPath, newPath: newPath);
}