using cCoder.Eventing;
using cCoder.Eventing.Models;
using FileEntity = cCoder.Data.Models.DMS.File;


namespace cCoder.DocumentManagement.Brokers.Events;

public class FileEventBroker(IEventHub eventHub) : IFileEventBroker
{
    public ValueTask RaiseFileAddEventAsync(EventMessage<FileEntity> message) =>
        eventHub.RaiseEventAsync("file_add", message);

    public ValueTask RaiseFileUpdateEventAsync(EventMessage<FileEntity> message) =>
        eventHub.RaiseEventAsync("file_update", message);

    public ValueTask RaiseFileDeleteEventAsync(EventMessage<FileEntity> message) =>
        eventHub.RaiseEventAsync("file_delete", message);
}






