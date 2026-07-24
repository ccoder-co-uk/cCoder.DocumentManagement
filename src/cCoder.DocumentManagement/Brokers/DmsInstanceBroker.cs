// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.DocumentManagement.Exposures;
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

internal sealed class DmsInstanceBroker(IDmsInstanceFactory dmsInstanceFactory) : IDmsInstanceBroker
{
    public DmsResult GetFilesZipped(IEnumerable<DmsPath> paths)
        =>
        dmsInstanceFactory.CreateDmsInstance()
                                         .GetFilesZipped(paths: paths);

    public DmsResult Get(DmsPath path, int version = 0, string search = "")
        =>
        dmsInstanceFactory.CreateDmsInstance()
                                         .Get(path: path, version: version, search: search);

    public IEnumerable<File> Search(string needle)
        =>
        dmsInstanceFactory.CreateDmsInstance()
                                         .Search(needle: needle);

    public ValueTask UnpackAsync(DmsPath path, Stream content, bool ignoreArchiveRoot = false)
        =>
        dmsInstanceFactory.CreateDmsInstance()
                                         .UnpackAsync(path: path, content: content, ignoreArchiveRoot: ignoreArchiveRoot);

    public ValueTask SaveAsync(DmsPath path, Stream content = null)
        =>
        dmsInstanceFactory.CreateDmsInstance()
                                         .SaveAsync(path: path, content: content);

    public ValueTask DropAsync(DmsPath path, int version = 0)
        =>
        dmsInstanceFactory.CreateDmsInstance()
                                         .DropAsync(path: path, version: version);

    public ValueTask CopyAsync(DmsPath oldPath, DmsPath newPath)
        =>
        dmsInstanceFactory.CreateDmsInstance()
                                         .CopyAsync(oldPath: oldPath, newPath: newPath);

    public ValueTask MoveAsync(DmsPath oldPath, DmsPath newPath)
        =>
        dmsInstanceFactory.CreateDmsInstance()
                                         .MoveAsync(oldPath: oldPath, newPath: newPath);
}