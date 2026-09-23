// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using DataFile = cCoder.Data.Models.DMS.File;

namespace cCoder.DocumentManagement.Exposures;

internal interface IFileMutationOperationsExposure
{
    IQueryable<DataFile> GetAllFiles(bool ignoreFilters = false);

    ValueTask DeleteFileAsync(Guid fileId);
}