// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using LocalFile = cCoder.Data.Models.DMS.File;


namespace cCoder.DocumentManagement.Services.Foundations.Events;

internal interface IFileEventService
{
    ValueTask RaiseFileAddEventAsync(LocalFile entity);
    ValueTask RaiseFileUpdateEventAsync(LocalFile entity);
    ValueTask RaiseFileDeleteEventAsync(LocalFile entity);
}