// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Foundations.Events;


namespace cCoder.DocumentManagement.Services.Processings;

internal partial class FileContentEventProcessingService(IFileContentEventService eventService) : IFileContentEventProcessingService
{
    public ValueTask RaiseFileContentAddEventAsync(FileContent entity)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [entity]);
            return eventService.RaiseFileContentAddEventAsync(entity: entity);
        });

    public ValueTask RaiseFileContentUpdateEventAsync(FileContent entity)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [entity]);
            return eventService.RaiseFileContentUpdateEventAsync(entity: entity);
        });

    public ValueTask RaiseFileContentDeleteEventAsync(FileContent entity)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [entity]);
            return eventService.RaiseFileContentDeleteEventAsync(entity: entity);
        });
}