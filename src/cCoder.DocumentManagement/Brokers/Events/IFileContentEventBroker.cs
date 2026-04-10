using cCoder.Data.Models.DMS;
using EventLibrary.Models;


namespace cCoder.DocumentManagement.Brokers.Events;

public interface IFileContentEventBroker
{
    ValueTask RaiseFileContentAddEventAsync(EventMessage<FileContent> message);
    ValueTask RaiseFileContentUpdateEventAsync(EventMessage<FileContent> message);
    ValueTask RaiseFileContentDeleteEventAsync(EventMessage<FileContent> message);
}







