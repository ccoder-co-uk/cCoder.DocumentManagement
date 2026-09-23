// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.DMS;

namespace cCoder.DocumentManagement.Exposures;

public interface IFolderEventManager
{
    ValueTask HandleFolderDeleteEventAsync(Folder folder);
}