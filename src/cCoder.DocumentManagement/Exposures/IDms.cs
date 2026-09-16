// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using DmsFile = cCoder.Data.Models.DMS.File;
using DmsPath = cCoder.DocumentManagement.Models.Path;
using DmsResult = cCoder.DocumentManagement.Models.DMSResult;


namespace cCoder.DocumentManagement.Exposures;

public interface IDms
{
    DmsResult GetFilesZipped(IEnumerable<DmsPath> paths);

    DmsResult GetFilesZipped(IEnumerable<string> paths);

    DmsResult Get(DmsPath path, int version = 0, string search = "");

    DmsResult Get(string path, int version = 0, string search = "");

    IEnumerable<DmsFile> Search(string needle);

    ValueTask UnpackAsync(DmsPath path, Stream content, bool ignoreArchiveRoot = false);

    ValueTask UnpackAsync(string path, Stream content, bool ignoreArchiveRoot = false);

    ValueTask SaveAsync(DmsPath path, Stream content = null);

    ValueTask SaveAsync(string path, Stream content = null);

    ValueTask DropAsync(DmsPath path, int version = 0);

    ValueTask DropAsync(string path, int version = 0);

    ValueTask CopyAsync(DmsPath oldPath, DmsPath newPath);

    ValueTask CopyAsync(string oldPath, string newPath);

    ValueTask MoveAsync(DmsPath oldPath, DmsPath newPath);

    ValueTask MoveAsync(string oldPath, string newPath);
}