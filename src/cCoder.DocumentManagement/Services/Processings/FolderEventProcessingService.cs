using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Foundations.Events;


namespace cCoder.DocumentManagement.Services.Processings;

internal class FolderEventProcessingService(IFolderEventService eventService) : IFolderEventProcessingService
{
    public ValueTask RaiseFolderAddEventAsync(Folder entity) => eventService.RaiseFolderAddEventAsync(entity);

    public ValueTask RaiseFolderUpdateEventAsync(Folder entity) => eventService.RaiseFolderUpdateEventAsync(entity);

    public ValueTask RaiseFolderDeleteEventAsync(Folder entity) => eventService.RaiseFolderDeleteEventAsync(entity);
}







