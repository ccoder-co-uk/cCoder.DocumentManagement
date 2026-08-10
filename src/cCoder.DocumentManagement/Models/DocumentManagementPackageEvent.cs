// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;

namespace cCoder.DocumentManagement.Models;

public sealed class DocumentManagementPackageEvent
{
    public int AppId { get; set; }

    public Package Package { get; set; }
}