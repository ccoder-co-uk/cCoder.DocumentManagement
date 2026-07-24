// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using DmsFile = cCoder.Data.Models.DMS.File;
using DmsPath = cCoder.DocumentManagement.Dependencies.Path;
using DmsResult = cCoder.DocumentManagement.Dependencies.DMSResult;


namespace cCoder.DocumentManagement.Exposures;

public interface IDms
{
    DmsResult GetFilesZipped(IEnumerable<DmsPath> paths);

    DmsResult Get(DmsPath path, int version = 0, string search = "");

    IEnumerable<DmsFile> Search(string needle);

    ValueTask UnpackAsync(DmsPath path, Stream content, bool ignoreArchiveRoot = false);

    ValueTask SaveAsync(DmsPath path, Stream content = null);

    ValueTask DropAsync(DmsPath path, int version = 0);

    ValueTask CopyAsync(DmsPath oldPath, DmsPath newPath);

    ValueTask MoveAsync(DmsPath oldPath, DmsPath newPath);
}