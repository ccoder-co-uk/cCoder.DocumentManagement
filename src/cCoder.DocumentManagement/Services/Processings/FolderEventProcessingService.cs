// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Foundations.Events;


namespace cCoder.DocumentManagement.Services.Processings;

internal partial class FolderEventProcessingService(IFolderEventService eventService) : IFolderEventProcessingService
{
    public ValueTask RaiseFolderAddEventAsync(Folder entity)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [entity]);
            return eventService.RaiseFolderAddEventAsync(entity: entity);
        });

    public ValueTask RaiseFolderUpdateEventAsync(Folder entity)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [entity]);
            return eventService.RaiseFolderUpdateEventAsync(entity: entity);
        });

    public ValueTask RaiseFolderDeleteEventAsync(Folder entity)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [entity]);
            return eventService.RaiseFolderDeleteEventAsync(entity: entity);
        });
}