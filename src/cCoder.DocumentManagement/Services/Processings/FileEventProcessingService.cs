// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Services.Foundations.Events;
using LocalFile = cCoder.Data.Models.DMS.File;


namespace cCoder.DocumentManagement.Services.Processings;

internal partial class FileEventProcessingService(IFileEventService eventService) : IFileEventProcessingService
{
    public ValueTask RaiseFileAddEventAsync(LocalFile entity)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [entity]);
            return eventService.RaiseFileAddEventAsync(entity: entity);
        });

    public ValueTask RaiseFileUpdateEventAsync(LocalFile entity)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [entity]);
            return eventService.RaiseFileUpdateEventAsync(entity: entity);
        });

    public ValueTask RaiseFileDeleteEventAsync(LocalFile entity)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [entity]);
            return eventService.RaiseFileDeleteEventAsync(entity: entity);
        });
}