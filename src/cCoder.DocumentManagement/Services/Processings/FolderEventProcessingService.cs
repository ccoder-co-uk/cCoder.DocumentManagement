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
    public ValueTask RaiseFolderAddEventAsync(Folder folder)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [folder]);
            return eventService.RaiseFolderAddEventAsync(folder: folder);
        });

    public ValueTask RaiseFolderUpdateEventAsync(Folder folder)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [folder]);
            return eventService.RaiseFolderUpdateEventAsync(folder: folder);
        });

    public ValueTask RaiseFolderDeleteEventAsync(Folder folder)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [folder]);
            return eventService.RaiseFolderDeleteEventAsync(folder: folder);
        });
}