using cCoder.Data.Models.DMS;
using EventLibrary.Models;


namespace cCoder.DocumentManagement.Brokers.Events;

public interface IFolderEventBroker
{
    ValueTask RaiseFolderAddEventAsync(EventMessage<Folder> message);
    ValueTask RaiseFolderUpdateEventAsync(EventMessage<Folder> message);
    ValueTask RaiseFolderDeleteEventAsync(EventMessage<Folder> message);
}







