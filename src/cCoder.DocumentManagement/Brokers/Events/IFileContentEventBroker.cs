// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;

using cCoder.Data.Models.DMS;
using cCoder.Eventing.Models;


namespace cCoder.DocumentManagement.Brokers.Events;

public interface IFileContentEventBroker
{
    ValueTask RaiseFileContentAddEventAsync(EventMessage<FileContent> message);
    ValueTask RaiseFileContentUpdateEventAsync(EventMessage<FileContent> message);
    ValueTask RaiseFileContentDeleteEventAsync(EventMessage<FileContent> message);
}