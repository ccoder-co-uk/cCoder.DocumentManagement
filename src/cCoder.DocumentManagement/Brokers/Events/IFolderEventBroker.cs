// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.DMS;
using cCoder.Eventing.Models;


namespace cCoder.DocumentManagement.Brokers.Events;

public interface IFolderEventBroker
{
    ValueTask RaiseFolderAddEventAsync(EventMessage<Folder> message);
    ValueTask RaiseFolderUpdateEventAsync(EventMessage<Folder> message);
    ValueTask RaiseFolderDeleteEventAsync(EventMessage<Folder> message);
}