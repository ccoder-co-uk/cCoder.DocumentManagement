// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;

using cCoder.Data.Models.DMS;

namespace cCoder.DocumentManagement.Exposures;

public interface IFolderEventManager
{
    ValueTask HandleFolderDeleteEventAsync(Folder folder);
}