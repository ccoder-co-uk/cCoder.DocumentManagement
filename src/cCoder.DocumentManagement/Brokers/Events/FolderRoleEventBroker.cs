using cCoder.Data.Models.Security;
using cCoder.Eventing;
using cCoder.Eventing.Models;


namespace cCoder.DocumentManagement.Brokers.Events;

public class FolderRoleEventBroker(IEventHub eventHub) : IFolderRoleEventBroker
{
    public ValueTask RaiseFolderRoleAddEventAsync(EventMessage<FolderRole> message) =>
        eventHub.RaiseEventAsync("folder_role_add", message);

    public ValueTask RaiseFolderRoleDeleteEventAsync(EventMessage<FolderRole> message) =>
        eventHub.RaiseEventAsync("folder_role_delete", message);
}







