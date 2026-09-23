// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.CodeAnalysis.Exposures;
using cCoder.DocumentManagement.Models;

namespace cCoder.DocumentManagement.Exposures;

public interface IDmsInstanceFactory
{
    IDms CreateDms();
    DMSResult GetFilesZipped(IEnumerable<string> paths);
    DMSResult Get(string path, int version = 0, string search = "");
    IEnumerable<cCoder.Data.Models.DMS.File> Search(string needle);
    ValueTask UnpackAsync(string path, Stream content, bool ignoreArchiveRoot = false);
    ValueTask SaveAsync(string path, Stream content = null);
    ValueTask DropAsync(string path, int version = 0);
    ValueTask CopyAsync(string oldPath, string newPath);
    ValueTask MoveAsync(string oldPath, string newPath);
}

public interface IDmsFactoryExposure
{
    DMSResult GetFilesZipped(IEnumerable<string> paths);
    DMSResult Get(string path, int version = 0, string search = "");
    IEnumerable<cCoder.Data.Models.DMS.File> Search(string needle);
    ValueTask UnpackAsync(string path, Stream content, bool ignoreArchiveRoot = false);
    ValueTask SaveAsync(string path, Stream content = null);
    ValueTask DropAsync(string path, int version = 0);
    ValueTask CopyAsync(string oldPath, string newPath);
    ValueTask MoveAsync(string oldPath, string newPath);
}

internal sealed class DmsInstanceFactory(
    IDms dms)
    : IDmsInstanceFactory, IDmsFactoryExposure, ICompositionExposure
{
    public IDms CreateDms() =>
        dms;

    public DMSResult GetFilesZipped(IEnumerable<string> paths) =>
        dms.GetFilesZipped(paths: paths);

    public DMSResult Get(string path, int version = 0, string search = "") =>
        dms.Get(path: path, version: version, search: search);

    public IEnumerable<cCoder.Data.Models.DMS.File> Search(string needle) =>
        dms.Search(needle: needle);

    public ValueTask UnpackAsync(string path, Stream content, bool ignoreArchiveRoot = false) =>
        dms.UnpackAsync(path: path, content: content, ignoreArchiveRoot: ignoreArchiveRoot);

    public ValueTask SaveAsync(string path, Stream content = null) =>
        dms.SaveAsync(path: path, content: content);

    public ValueTask DropAsync(string path, int version = 0) =>
        dms.DropAsync(path: path, version: version);

    public ValueTask CopyAsync(string oldPath, string newPath) =>
        dms.CopyAsync(oldPath: oldPath, newPath: newPath);

    public ValueTask MoveAsync(string oldPath, string newPath) =>
        dms.MoveAsync(oldPath: oldPath, newPath: newPath);
}