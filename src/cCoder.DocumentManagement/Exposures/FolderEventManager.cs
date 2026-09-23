// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.DMS;
using cCoder.DocumentManagement.Services.Aggregations;

namespace cCoder.DocumentManagement.Exposures;

internal sealed class FolderEventManager(
    IFolderMutationAggregationService folderMutationAggregationService)
    : IFolderEventManager
{
    public ValueTask HandleFolderDeleteEventAsync(Folder folder) =>
        folderMutationAggregationService.HandleFolderDeleteEventAsync(folder: folder);
}