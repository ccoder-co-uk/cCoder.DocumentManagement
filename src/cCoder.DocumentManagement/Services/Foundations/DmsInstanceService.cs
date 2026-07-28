// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Brokers;
using DmsPath = cCoder.DocumentManagement.Models.Path;
using DmsResult = cCoder.DocumentManagement.Models.DMSResult;


namespace cCoder.DocumentManagement.Services.Foundations;

internal partial class DmsInstanceService(IDmsInstanceBroker dmsInstanceBroker) : IDmsInstanceService
{
    public DmsResult GetFilesZipped(IEnumerable<string> paths) =>
        TryCatch(operation: () =>
        {
            ValidateFilesZippedOnGet(paths: paths);

            return dmsInstanceBroker.GetFilesZipped(
                paths: paths.Select(
                    selector: path =>
                        new DmsPath(path: path)));
        });

    public DmsResult Get(string path, int version = 0, string search = "") =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [path, version, search]);

            return dmsInstanceBroker.Get(
                path: new DmsPath(path: path),
                version: version,
                search: search);
        });

    public ValueTask UnpackAsync(string path, Stream content, bool ignoreArchiveRoot = false) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [path, content, ignoreArchiveRoot]);

            return dmsInstanceBroker.UnpackAsync(
                path: new DmsPath(path: path),
                content: content,
                ignoreArchiveRoot: ignoreArchiveRoot);
        });

    public ValueTask SaveAsync(string path, Stream content = null) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [path, content]);

            return dmsInstanceBroker.SaveAsync(
                path: new DmsPath(path: path),
                content: content);
        });

    public ValueTask DropAsync(string path, int version = 0) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [path, version]);

            return dmsInstanceBroker.DropAsync(
                path: new DmsPath(path: path),
                version: version);
        });

    public ValueTask CopyAsync(string oldPath, string newPath) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [oldPath, newPath]);

            return dmsInstanceBroker.CopyAsync(
                oldPath: new DmsPath(path: oldPath),
                newPath: new DmsPath(path: newPath));
        });

    public ValueTask MoveAsync(string oldPath, string newPath) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [oldPath, newPath]);

            return dmsInstanceBroker.MoveAsync(
                oldPath: new DmsPath(path: oldPath),
                newPath: new DmsPath(path: newPath));
        });
}