// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.DMS;

namespace cCoder.DocumentManagement.Models;

internal sealed class FolderArchiveData
{
    public ILookup<Guid?, Folder> SubFoldersByParentId { get; init; }
    public ILookup<Guid, cCoder.Data.Models.DMS.File> FilesByFolderId { get; init; }
    public ILookup<Guid, FileContent> FileContentsByFileId { get; init; }
}