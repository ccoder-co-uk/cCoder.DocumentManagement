// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;

using LocalFile = cCoder.Data.Models.DMS.File;


namespace cCoder.DocumentManagement.Services.Foundations.Events;

internal interface IFileEventService
{
    ValueTask RaiseFileAddEventAsync(LocalFile entity);
    ValueTask RaiseFileUpdateEventAsync(LocalFile entity);
    ValueTask RaiseFileDeleteEventAsync(LocalFile entity);
}