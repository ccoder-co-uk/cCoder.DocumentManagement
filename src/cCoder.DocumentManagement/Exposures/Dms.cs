using cCoder.DocumentManagement.Services.Orchestrations;
using DmsFile = cCoder.Data.Models.DMS.File;
using DmsPath = cCoder.DocumentManagement.Models.Path;
using DmsResult = cCoder.DocumentManagement.Models.DMSResult;


namespace cCoder.DocumentManagement.Exposures;

public class Dms(IDmsOrchestrationService dmsOrchestrationService) : IDms
{
    public DmsResult GetFilesZipped(IEnumerable<DmsPath> paths) =>
        dmsOrchestrationService.GetFilesZipped(paths);

    public DmsResult Get(DmsPath path, int version = 0, string search = "") =>
        dmsOrchestrationService.Get(path, version, search);

    public IEnumerable<DmsFile> Search(string needle) => dmsOrchestrationService.Search(needle);

    public ValueTask UnpackAsync(DmsPath path, Stream content, bool ignoreArchiveRoot = false) =>
        dmsOrchestrationService.UnpackAsync(path, content, ignoreArchiveRoot);

    public ValueTask SaveAsync(DmsPath path, Stream content = null) =>
        dmsOrchestrationService.SaveAsync(path, content);

    public ValueTask DropAsync(DmsPath path, int version = 0) =>
        dmsOrchestrationService.DropAsync(path, version);

    public ValueTask CopyAsync(DmsPath oldPath, DmsPath newPath) =>
        dmsOrchestrationService.CopyAsync(oldPath, newPath);

    public ValueTask MoveAsync(DmsPath oldPath, DmsPath newPath) =>
        dmsOrchestrationService.MoveAsync(oldPath, newPath);
}
