using cCoder.DocumentManagement.Models;
using File = cCoder.Data.Models.DMS.File;
using DmsPath = cCoder.DocumentManagement.Models.Path;
using DmsResult = cCoder.DocumentManagement.Models.DMSResult;


namespace cCoder.DocumentManagement.Brokers;

public interface IDmsInstanceBroker
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

public class DmsInstanceBroker(IDmsInstanceFactory dmsInstanceFactory) : IDmsInstanceBroker
{
    public DmsResult GetFilesZipped(IEnumerable<DmsPath> paths)
        => dmsInstanceFactory.CreateDms().GetFilesZipped(paths);

    public DmsResult Get(DmsPath path, int version = 0, string search = "")
        => dmsInstanceFactory.CreateDms().Get(path, version, search);

    public IEnumerable<File> Search(string needle)
        => dmsInstanceFactory.CreateDms().Search(needle);

    public ValueTask UnpackAsync(DmsPath path, Stream content, bool ignoreArchiveRoot = false)
        => dmsInstanceFactory.CreateDms().UnpackAsync(path, content, ignoreArchiveRoot);

    public ValueTask SaveAsync(DmsPath path, Stream content = null)
        => dmsInstanceFactory.CreateDms().SaveAsync(path, content);

    public ValueTask DropAsync(DmsPath path, int version = 0)
        => dmsInstanceFactory.CreateDms().DropAsync(path, version);

    public ValueTask CopyAsync(DmsPath oldPath, DmsPath newPath)
        => dmsInstanceFactory.CreateDms().CopyAsync(oldPath, newPath);

    public ValueTask MoveAsync(DmsPath oldPath, DmsPath newPath)
        => dmsInstanceFactory.CreateDms().MoveAsync(oldPath, newPath);
}







