// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.DMS;

namespace cCoder.DocumentManagement.Services.Coordinations;

public interface IFolderCoordinationService
{
    ValueTask DeleteFolderAsync(Folder folder);
}