// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;

using cCoder.DocumentManagement.Models;


namespace cCoder.DocumentManagement.Exposures;

public interface IDocumentManagementPackageManager
{
    ValueTask ImportPackageAsync(int appId, DocumentManagementPackage documentManagementPackage);

    DocumentManagementPackage ExportPackage(int appId, string packageName);
}