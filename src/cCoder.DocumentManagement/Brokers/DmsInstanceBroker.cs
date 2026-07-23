// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        =>
        dmsInstanceFactory.CreateDms()
                                         .GetFilesZipped(paths: paths);

    public DmsResult Get(DmsPath path, int version = 0, string search = "")
        =>
        dmsInstanceFactory.CreateDms()
                                         .Get(path: path, version: version, search: search);

    public IEnumerable<File> Search(string needle)
        =>
        dmsInstanceFactory.CreateDms()
                                         .Search(needle: needle);

    public ValueTask UnpackAsync(DmsPath path, Stream content, bool ignoreArchiveRoot = false)
        =>
        dmsInstanceFactory.CreateDms()
                                         .UnpackAsync(path: path, content: content, ignoreArchiveRoot: ignoreArchiveRoot);

    public ValueTask SaveAsync(DmsPath path, Stream content = null)
        =>
        dmsInstanceFactory.CreateDms()
                                         .SaveAsync(path: path, content: content);

    public ValueTask DropAsync(DmsPath path, int version = 0)
        =>
        dmsInstanceFactory.CreateDms()
                                         .DropAsync(path: path, version: version);

    public ValueTask CopyAsync(DmsPath oldPath, DmsPath newPath)
        =>
        dmsInstanceFactory.CreateDms()
                                         .CopyAsync(oldPath: oldPath, newPath: newPath);

    public ValueTask MoveAsync(DmsPath oldPath, DmsPath newPath)
        =>
        dmsInstanceFactory.CreateDms()
                                         .MoveAsync(oldPath: oldPath, newPath: newPath);
}