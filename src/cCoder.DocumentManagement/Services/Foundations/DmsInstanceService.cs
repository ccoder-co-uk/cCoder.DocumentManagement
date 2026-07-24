// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Brokers;
using File = cCoder.Data.Models.DMS.File;
using DmsPath = cCoder.DocumentManagement.Models.Path;
using DmsResult = cCoder.DocumentManagement.Models.DMSResult;


namespace cCoder.DocumentManagement.Services.Foundations;

internal partial class DmsInstanceService(IDmsInstanceBroker dmsInstanceBroker) : IDmsInstanceService
{
    public DmsResult GetFilesZipped(IEnumerable<DmsPath> paths)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [paths]);
            return dmsInstanceBroker.GetFilesZipped(paths: paths);
        });

    public DmsResult Get(DmsPath path, int version = 0, string search = "")
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [path, version, search]);
            return dmsInstanceBroker.Get(path: path, version: version, search: search);
        });

    public IEnumerable<File> Search(string needle)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [needle]);
            return dmsInstanceBroker.Search(needle: needle);
        });

    public ValueTask UnpackAsync(DmsPath path, Stream content, bool ignoreArchiveRoot = false)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [path, content, ignoreArchiveRoot]);
            return dmsInstanceBroker.UnpackAsync(path: path, content: content, ignoreArchiveRoot: ignoreArchiveRoot);
        });

    public ValueTask SaveAsync(DmsPath path, Stream content = null)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [path, content]);
            return dmsInstanceBroker.SaveAsync(path: path, content: content);
        });

    public ValueTask DropAsync(DmsPath path, int version = 0)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [path, version]);
            return dmsInstanceBroker.DropAsync(path: path, version: version);
        });

    public ValueTask CopyAsync(DmsPath oldPath, DmsPath newPath)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [oldPath, newPath]);
            return dmsInstanceBroker.CopyAsync(oldPath: oldPath, newPath: newPath);
        });

    public ValueTask MoveAsync(DmsPath oldPath, DmsPath newPath)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [oldPath, newPath]);
            return dmsInstanceBroker.MoveAsync(oldPath: oldPath, newPath: newPath);
        });
}