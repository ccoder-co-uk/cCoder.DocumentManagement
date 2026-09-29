// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;

using cCoder.Data.Models.DMS;
using cCoder.DocumentManagement.Services.Aggregations;

namespace cCoder.DocumentManagement.Exposures;

internal sealed class FolderEventManager(
    IFolderMutationAggregationService folderMutationAggregationService)
    : IFolderEventManager,
      cCoder.CodeAnalysis.Exposures.ICompositionExposure
{
    public ValueTask HandleFolderDeleteEventAsync(Folder folder) =>
        folderMutationAggregationService.HandleFolderDeleteEventAsync(folder: folder);
}