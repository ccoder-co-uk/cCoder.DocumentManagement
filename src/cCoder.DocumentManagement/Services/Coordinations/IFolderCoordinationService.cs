// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.DMS;

namespace cCoder.DocumentManagement.Services.Coordinations;

internal interface IFolderCoordinationService
{
    ValueTask DeleteFolderAsync(Folder deletedFolder);
}