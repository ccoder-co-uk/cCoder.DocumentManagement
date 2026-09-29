// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.Security;


namespace cCoder.DocumentManagement.Services.Foundations.Events;

internal interface IFolderRoleEventService
{
    ValueTask RaiseFolderRoleAddEventAsync(FolderRole folderRole);
    ValueTask RaiseFolderRoleDeleteEventAsync(FolderRole folderRole);
}