// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.DMS;
using cCoder.Eventing;
using cCoder.Eventing.Models;


namespace cCoder.DocumentManagement.Brokers.Events;

internal sealed class FolderEventBroker(IEventHub eventHub) : IFolderEventBroker
{
    public ValueTask RaiseFolderAddEventAsync(EventMessage<Folder> message) =>
        eventHub.RaiseEventAsync(name: "folder_add", message: message);

    public ValueTask RaiseFolderUpdateEventAsync(EventMessage<Folder> message) =>
        eventHub.RaiseEventAsync(name: "folder_update", message: message);

    public ValueTask RaiseFolderDeleteEventAsync(EventMessage<Folder> message) =>
        eventHub.RaiseEventAsync(name: "folder_delete", message: message);
}