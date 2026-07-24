// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using DataFile = cCoder.Data.Models.DMS.File;

namespace cCoder.DocumentManagement.Exposures;

public interface IFileOperationsExposure
{
    IQueryable<DataFile> GetAllFiles(bool ignoreFilters = false);

    DataFile GetFileByPathWithFolderAndContents(
        int appId,
        string path);

    Guid[] GetFileIdsByFolderIds(
        IEnumerable<Guid> folderIds,
        bool ignoreFilters);

    ValueTask<DataFile> UpdateFileAsync(DataFile updatedFile);

    ValueTask<DataFile> UpdateFileForAppAsync(DataFile updatedFile);

}