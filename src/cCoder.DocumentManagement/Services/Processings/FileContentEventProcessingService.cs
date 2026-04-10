using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Foundations.Events;


namespace cCoder.DocumentManagement.Services.Processings;

internal class FileContentEventProcessingService(IFileContentEventService eventService) : IFileContentEventProcessingService
{
    public ValueTask RaiseFileContentAddEventAsync(FileContent entity) => eventService.RaiseFileContentAddEventAsync(entity);

    public ValueTask RaiseFileContentUpdateEventAsync(FileContent entity) => eventService.RaiseFileContentUpdateEventAsync(entity);

    public ValueTask RaiseFileContentDeleteEventAsync(FileContent entity) => eventService.RaiseFileContentDeleteEventAsync(entity);
}







