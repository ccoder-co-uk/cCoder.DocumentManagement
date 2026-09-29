// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.DMS;


namespace cCoder.DocumentManagement.Services.Foundations.Events;

internal interface IFolderEventService
{
    ValueTask RaiseFolderAddEventAsync(Folder folder);
    ValueTask RaiseFolderUpdateEventAsync(Folder folder);
    ValueTask RaiseFolderDeleteEventAsync(Folder folder);
}