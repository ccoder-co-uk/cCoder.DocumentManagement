// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Services.Aggregations;
using DataFile = cCoder.Data.Models.DMS.File;

namespace cCoder.DocumentManagement.Exposures;

internal sealed class FileMutationOperationsExposure(
    IFileMutationAggregationService fileMutationAggregationService)
    : IFileMutationOperationsExposure,
      cCoder.CodeAnalysis.Exposures.ICompositionExposure
{
    public IQueryable<DataFile> GetAllFiles(bool ignoreFilters = false) =>
        fileMutationAggregationService.GetAll(ignoreFilters: ignoreFilters);

    public ValueTask DeleteFileAsync(Guid fileId) =>
        fileMutationAggregationService.DeleteAsync(fileId: fileId);
}