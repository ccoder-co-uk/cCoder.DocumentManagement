using File = cCoder.Data.Models.DMS.File;
using DmsPath = cCoder.DocumentManagement.Models.Path;
using DmsResult = cCoder.DocumentManagement.Models.DMSResult;


namespace cCoder.DocumentManagement.Services.Foundations;

public interface IDmsInstanceService
{
    DmsResult GetFilesZipped(IEnumerable<DmsPath> paths);
    DmsResult Get(DmsPath path, int version = 0, string search = "");
    IEnumerable<File> Search(string needle);
    ValueTask UnpackAsync(DmsPath path, Stream content, bool ignoreArchiveRoot = false);
    ValueTask SaveAsync(DmsPath path, Stream content = null);
    ValueTask DropAsync(DmsPath path, int version = 0);
    ValueTask CopyAsync(DmsPath oldPath, DmsPath newPath);
    ValueTask MoveAsync(DmsPath oldPath, DmsPath newPath);
}










