// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing;
using cCoder.Eventing.Models;
using FileEntity = cCoder.Data.Models.DMS.File;


namespace cCoder.DocumentManagement.Brokers.Events;

internal sealed class FileEventBroker(IEventHub eventHub) : IFileEventBroker
{
    public ValueTask RaiseFileAddEventAsync(EventMessage<FileEntity> message) =>
        eventHub.RaiseEventAsync(name: "file_add", message: message);

    public ValueTask RaiseFileUpdateEventAsync(EventMessage<FileEntity> message) =>
        eventHub.RaiseEventAsync(name: "file_update", message: message);

    public ValueTask RaiseFileDeleteEventAsync(EventMessage<FileEntity> message) =>
        eventHub.RaiseEventAsync(name: "file_delete", message: message);
}