// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using cCoder.Eventing;
using cCoder.Eventing.Models;


namespace cCoder.DocumentManagement.Brokers.Events;

public class FolderRoleEventBroker(IEventHub eventHub) : IFolderRoleEventBroker
{
    public ValueTask RaiseFolderRoleAddEventAsync(EventMessage<FolderRole> message) =>
        eventHub.RaiseEventAsync(name: "folder_role_add", message: message);

    public ValueTask RaiseFolderRoleDeleteEventAsync(EventMessage<FolderRole> message) =>
        eventHub.RaiseEventAsync(name: "folder_role_delete", message: message);
}