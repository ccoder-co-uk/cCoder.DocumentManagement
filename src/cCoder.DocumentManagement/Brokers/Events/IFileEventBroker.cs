using EventLibrary.Models;
using FileEntity = cCoder.Data.Models.DMS.File;


namespace cCoder.DocumentManagement.Brokers.Events;

public interface IFileEventBroker
{
    ValueTask RaiseFileAddEventAsync(EventMessage<FileEntity> message);
    ValueTask RaiseFileUpdateEventAsync(EventMessage<FileEntity> message);
    ValueTask RaiseFileDeleteEventAsync(EventMessage<FileEntity> message);
}






