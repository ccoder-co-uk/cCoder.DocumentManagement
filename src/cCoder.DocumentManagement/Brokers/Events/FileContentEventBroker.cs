using cCoder.Data.Models.DMS;
using EventLibrary;
using EventLibrary.Models;


namespace cCoder.DocumentManagement.Brokers.Events;

public class FileContentEventBroker(IEventHub eventHub) : IFileContentEventBroker
{
    public ValueTask RaiseFileContentAddEventAsync(EventMessage<FileContent> message) =>
        eventHub.RaiseEventAsync("file_content_add", message);

    public ValueTask RaiseFileContentUpdateEventAsync(EventMessage<FileContent> message) =>
        eventHub.RaiseEventAsync("file_content_update", message);

    public ValueTask RaiseFileContentDeleteEventAsync(EventMessage<FileContent> message) =>
        eventHub.RaiseEventAsync("file_content_delete", message);
}







