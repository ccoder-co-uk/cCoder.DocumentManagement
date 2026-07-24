// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;

namespace cCoder.DocumentManagement.Models;

public sealed class FolderRoleContext
{
    public Folder Folder { get; set; }

    public Role Role { get; set; }
}