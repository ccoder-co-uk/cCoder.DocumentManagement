// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;


namespace cCoder.DocumentManagement.Services.Foundations.Events;

internal interface IFolderEventService
{
    ValueTask RaiseFolderAddEventAsync(Folder folder);
    ValueTask RaiseFolderUpdateEventAsync(Folder folder);
    ValueTask RaiseFolderDeleteEventAsync(Folder folder);
}