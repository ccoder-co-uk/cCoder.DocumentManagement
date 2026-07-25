// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using LocalFile = cCoder.Data.Models.DMS.File;


namespace cCoder.DocumentManagement.Services.Processings;

public interface IFileEventProcessingService
{
    ValueTask RaiseFileAddEventAsync(LocalFile entity);
    ValueTask RaiseFileUpdateEventAsync(LocalFile entity);
    ValueTask RaiseFileDeleteEventAsync(LocalFile entity);
}