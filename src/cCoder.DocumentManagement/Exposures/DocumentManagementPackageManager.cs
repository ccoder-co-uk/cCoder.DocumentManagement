// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Aggregations;


namespace cCoder.DocumentManagement.Exposures;

internal class DocumentManagementPackageManager(
    IDocumentManagementMigrationAggregationService documentManagementMigrationAggregationService
) : IDocumentManagementPackageManager
{
    public ValueTask ImportPackageAsync(int appId, DocumentManagementPackage package) =>
        documentManagementMigrationAggregationService.ImportPackageDocumentManagementPackageAsync(appId: appId, package: package);

    public DocumentManagementPackage ExportPackage(int appId, string packageName) =>
        documentManagementMigrationAggregationService.ExportPackage(appId: appId, packageName: packageName);
}