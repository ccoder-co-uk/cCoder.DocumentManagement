// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Foundations.Events;


namespace cCoder.DocumentManagement.Services.Processings;

internal partial class FolderRoleEventProcessingService(IFolderRoleEventService eventService) : IFolderRoleEventProcessingService
{
    public ValueTask RaiseFolderRoleAddEventAsync(FolderRole entity)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [entity]);
            return eventService.RaiseFolderRoleAddEventAsync(entity: entity);
        });

    public ValueTask RaiseFolderRoleDeleteEventAsync(FolderRole entity)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [entity]);
            return eventService.RaiseFolderRoleDeleteEventAsync(entity: entity);
        });
}