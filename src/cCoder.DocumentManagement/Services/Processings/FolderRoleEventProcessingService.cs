// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Foundations.Events;


namespace cCoder.DocumentManagement.Services.Processings;

internal class FolderRoleEventProcessingService(IFolderRoleEventService eventService) : IFolderRoleEventProcessingService
{
    public ValueTask RaiseFolderRoleAddEventAsync(FolderRole entity) =>
        eventService.RaiseFolderRoleAddEventAsync(entity: entity);

    public ValueTask RaiseFolderRoleDeleteEventAsync(FolderRole entity) =>
        eventService.RaiseFolderRoleDeleteEventAsync(entity: entity);
}