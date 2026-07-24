// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.DMS;

namespace cCoder.DocumentManagement.Models;

internal sealed record FolderArchiveData(
    ILookup<Guid?, Folder> SubFoldersByParentId,
    ILookup<Guid, cCoder.Data.Models.DMS.File> FilesByFolderId,
    ILookup<Guid, FileContent> FileContentsByFileId);
