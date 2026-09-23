// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using DmsPath = cCoder.DocumentManagement.Models.Path;
using DmsResult = cCoder.DocumentManagement.Models.DMSResult;

namespace cCoder.DocumentManagement.Exposures;

public interface IDmsInstanceOperationsExposure
{
    DmsResult Get(string path, int version = 0, string search = "");
    ValueTask UnpackAsync(string path, Stream content, bool ignoreArchiveRoot = false);
    ValueTask SaveAsync(string path, Stream content = null);
    ValueTask DropAsync(string path, int version = 0);
    ValueTask CopyAsync(string oldPath, string newPath);
    ValueTask MoveAsync(string oldPath, string newPath);

    DmsResult GetDmsPath(
        DmsPath path,
        int version = 0,
        string search = "");

    DmsResult GetFilesZipped(IEnumerable<string> paths);

    ValueTask UnpackDmsPathAsync(
        DmsPath path,
        Stream content,
        bool ignoreArchiveRoot = false);

    ValueTask SaveDmsPathAsync(
        DmsPath path,
        Stream content = null);

    ValueTask MoveDmsPathAsync(
        DmsPath oldPath,
        DmsPath newPath);

    ValueTask CopyDmsPathAsync(
        DmsPath oldPath,
        DmsPath newPath);

    ValueTask DropDmsPathAsync(
        DmsPath path,
        int version = 0);
}