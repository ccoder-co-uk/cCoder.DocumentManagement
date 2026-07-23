// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Services.Foundations.Events;
using LocalFile = cCoder.Data.Models.DMS.File;


namespace cCoder.DocumentManagement.Services.Processings;

internal class FileEventProcessingService(IFileEventService eventService) : IFileEventProcessingService
{
    public ValueTask RaiseFileAddEventAsync(LocalFile entity) =>
        eventService.RaiseFileAddEventAsync(entity: entity);

    public ValueTask RaiseFileUpdateEventAsync(LocalFile entity) =>
        eventService.RaiseFileUpdateEventAsync(entity: entity);

    public ValueTask RaiseFileDeleteEventAsync(LocalFile entity) =>
        eventService.RaiseFileDeleteEventAsync(entity: entity);
}