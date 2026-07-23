// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.DMS;
using cCoder.Eventing;
using cCoder.Eventing.Models;


namespace cCoder.DocumentManagement.Brokers.Events;

public class FileContentEventBroker(IEventHub eventHub) : IFileContentEventBroker
{
    public ValueTask RaiseFileContentAddEventAsync(EventMessage<FileContent> message) =>
        eventHub.RaiseEventAsync(name: "file_content_add", message: message);

    public ValueTask RaiseFileContentUpdateEventAsync(EventMessage<FileContent> message) =>
        eventHub.RaiseEventAsync(name: "file_content_update", message: message);

    public ValueTask RaiseFileContentDeleteEventAsync(EventMessage<FileContent> message) =>
        eventHub.RaiseEventAsync(name: "file_content_delete", message: message);
}