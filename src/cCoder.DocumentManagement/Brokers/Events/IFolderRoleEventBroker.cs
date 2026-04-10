using cCoder.Data.Models.Security;
using EventLibrary.Models;


namespace cCoder.DocumentManagement.Brokers.Events;

public interface IFolderRoleEventBroker
{
    ValueTask RaiseFolderRoleAddEventAsync(EventMessage<FolderRole> message);
    ValueTask RaiseFolderRoleDeleteEventAsync(EventMessage<FolderRole> message);
}







