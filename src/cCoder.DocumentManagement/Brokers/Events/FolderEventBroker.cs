using cCoder.Data.Models.DMS;
using EventLibrary;
using EventLibrary.Models;


namespace cCoder.DocumentManagement.Brokers.Events;

public class FolderEventBroker(IEventHub eventHub) : IFolderEventBroker
{
    public ValueTask RaiseFolderAddEventAsync(EventMessage<Folder> message) =>
        eventHub.RaiseEventAsync("folder_add", message);

    public ValueTask RaiseFolderUpdateEventAsync(EventMessage<Folder> message) =>
        eventHub.RaiseEventAsync("folder_update", message);

    public ValueTask RaiseFolderDeleteEventAsync(EventMessage<Folder> message) =>
        eventHub.RaiseEventAsync("folder_delete", message);
}







