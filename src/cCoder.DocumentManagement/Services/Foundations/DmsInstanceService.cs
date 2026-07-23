// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Brokers;
using File = cCoder.Data.Models.DMS.File;
using DmsPath = cCoder.DocumentManagement.Models.Path;
using DmsResult = cCoder.DocumentManagement.Models.DMSResult;


namespace cCoder.DocumentManagement.Services.Foundations;

internal class DmsInstanceService(IDmsInstanceBroker dmsInstanceBroker) : IDmsInstanceService
{
    public DmsResult GetFilesZipped(IEnumerable<DmsPath> paths) =>
        dmsInstanceBroker.GetFilesZipped(paths: paths);

    public DmsResult Get(DmsPath path, int version = 0, string search = "") =>
        dmsInstanceBroker.Get(path: path, version: version, search: search);

    public IEnumerable<File> Search(string needle) => dmsInstanceBroker.Search(needle: needle);

    public ValueTask UnpackAsync(DmsPath path, Stream content, bool ignoreArchiveRoot = false) =>
        dmsInstanceBroker.UnpackAsync(path: path, content: content, ignoreArchiveRoot: ignoreArchiveRoot);

    public ValueTask SaveAsync(DmsPath path, Stream content = null) =>
        dmsInstanceBroker.SaveAsync(path: path, content: content);

    public ValueTask DropAsync(DmsPath path, int version = 0) =>
        dmsInstanceBroker.DropAsync(path: path, version: version);

    public ValueTask CopyAsync(DmsPath oldPath, DmsPath newPath) =>
        dmsInstanceBroker.CopyAsync(oldPath: oldPath, newPath: newPath);

    public ValueTask MoveAsync(DmsPath oldPath, DmsPath newPath) =>
        dmsInstanceBroker.MoveAsync(oldPath: oldPath, newPath: newPath);
}