// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using DmsResult = cCoder.DocumentManagement.Models.DMSResult;


namespace cCoder.DocumentManagement.Services.Foundations;

internal interface IDmsInstanceService
{
    DmsResult GetFilesZipped(IEnumerable<string> paths);
    DmsResult Get(string path, int version = 0, string search = "");
    ValueTask UnpackAsync(string path, Stream content, bool ignoreArchiveRoot = false);
    ValueTask SaveAsync(string path, Stream content = null);
    ValueTask DropAsync(string path, int version = 0);
    ValueTask CopyAsync(string oldPath, string newPath);
    ValueTask MoveAsync(string oldPath, string newPath);
}