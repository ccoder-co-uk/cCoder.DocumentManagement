// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;

namespace cCoder.DocumentManagement.Exposures.Setup;

public static partial class UIBaseline
{
    static Package PageRoles => new()
    {
        Name = "Document Management Page Roles",
        Category = "DMS",
        Description = "Document Management Page Roles.",
        SourceApi = "https://ccoder.co.uk/Api/",
        Items =
        [
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Admin/DocumentManagement",
  "Role": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Admin/DocumentManagement",
  "Role": "Users"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Documentation/CoreDocumentation/DocumentManagementSystem",
  "Role": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Documentation/CoreDocumentation/DocumentManagementSystem",
  "Role": "Users"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Documentation/CoreDocumentation/DocumentManagementSystem",
  "Role": "Guests"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Documentation/CoreDocumentation/DocumentManagementSystem/SendingFilestoDMS",
  "Role": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Documentation/CoreDocumentation/DocumentManagementSystem/SendingFilestoDMS",
  "Role": "Users"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Documentation/CoreDocumentation/DocumentManagementSystem/SendingFilestoDMS",
  "Role": "Guests"
}
"""
            }
        ]
    };
}