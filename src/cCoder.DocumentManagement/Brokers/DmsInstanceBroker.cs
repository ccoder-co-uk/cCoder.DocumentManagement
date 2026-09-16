// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.DocumentManagement.Exposures;
using File = cCoder.Data.Models.DMS.File;
using DmsResult = cCoder.DocumentManagement.Models.DMSResult;


namespace cCoder.DocumentManagement.Brokers;

public interface IDmsInstanceBroker
{
    DmsResult GetFilesZipped(IEnumerable<string> paths);
    DmsResult Get(string path, int version = 0, string search = "");
    IEnumerable<File> Search(string needle);
    ValueTask UnpackAsync(string path, Stream content, bool ignoreArchiveRoot = false);
    ValueTask SaveAsync(string path, Stream content = null);
    ValueTask DropAsync(string path, int version = 0);
    ValueTask CopyAsync(string oldPath, string newPath);
    ValueTask MoveAsync(string oldPath, string newPath);
}

internal sealed class DmsInstanceBroker(IDmsInstanceFactory dmsInstanceFactory) : IDmsInstanceBroker
{
    public DmsResult GetFilesZipped(IEnumerable<string> paths)
        =>
        dmsInstanceFactory.CreateDms()
            .GetFilesZipped(paths: paths);

    public DmsResult Get(string path, int version = 0, string search = "")
        =>
        dmsInstanceFactory.CreateDms()
            .Get(path: path, version: version, search: search);

    public IEnumerable<File> Search(string needle)
        =>
        dmsInstanceFactory.CreateDms()
                                         .Search(needle: needle);

    public ValueTask UnpackAsync(string path, Stream content, bool ignoreArchiveRoot = false)
        =>
        dmsInstanceFactory.CreateDms()
            .UnpackAsync(path: path, content: content, ignoreArchiveRoot: ignoreArchiveRoot);

    public ValueTask SaveAsync(string path, Stream content = null)
        =>
        dmsInstanceFactory.CreateDms()
            .SaveAsync(path: path, content: content);

    public ValueTask DropAsync(string path, int version = 0)
        =>
        dmsInstanceFactory.CreateDms()
            .DropAsync(path: path, version: version);

    public ValueTask CopyAsync(string oldPath, string newPath)
        =>
        dmsInstanceFactory.CreateDms()
            .CopyAsync(oldPath: oldPath, newPath: newPath);

    public ValueTask MoveAsync(string oldPath, string newPath)
        =>
        dmsInstanceFactory.CreateDms()
            .MoveAsync(oldPath: oldPath, newPath: newPath);
}