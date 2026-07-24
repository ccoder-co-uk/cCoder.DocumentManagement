// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;


namespace cCoder.DocumentManagement.Services.Aggregations;

public interface IDocumentManagementMigrationAggregationService
{
    ValueTask ImportPackageDocumentManagementPackageAsync(int appId, DocumentManagementPackage package);

    DocumentManagementPackage ExportPackage(int appId, string packageName);
}