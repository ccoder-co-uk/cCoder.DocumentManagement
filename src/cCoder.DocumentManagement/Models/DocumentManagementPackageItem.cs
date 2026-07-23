// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.DocumentManagement.Models;

public class DocumentManagementPackageItem
{
    public Guid Id { get; set; }

    public Guid PackageId { get; set; }

    public string Type { get; set; }

    public string Data { get; set; }

    public virtual DocumentManagementPackage Package { get; set; }
}