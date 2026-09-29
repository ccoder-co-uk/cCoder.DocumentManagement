// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.Security;


namespace cCoder.DocumentManagement.Services.Processings;

internal interface IFolderRoleEventProcessingService
{
    ValueTask RaiseFolderRoleAddEventAsync(FolderRole folderRole);
    ValueTask RaiseFolderRoleDeleteEventAsync(FolderRole folderRole);
}