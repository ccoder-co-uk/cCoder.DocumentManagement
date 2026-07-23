// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using cCoder.Eventing.Models;


namespace cCoder.DocumentManagement.Brokers.Events;

public interface IFolderRoleEventBroker
{
    ValueTask RaiseFolderRoleAddEventAsync(EventMessage<FolderRole> message);
    ValueTask RaiseFolderRoleDeleteEventAsync(EventMessage<FolderRole> message);
}