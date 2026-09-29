// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;

using cCoder.DocumentManagement.Models;
using cCoder.DocumentManagement.Services.Aggregations;


namespace cCoder.DocumentManagement.Exposures;

internal class DocumentManagementPackageManager(
    IDocumentManagementMigrationAggregationService documentManagementMigrationAggregationService
) : IDocumentManagementPackageManager,
    cCoder.CodeAnalysis.Exposures.ICompositionExposure
{
    public ValueTask ImportPackageAsync(int appId, DocumentManagementPackage documentManagementPackage) =>
        documentManagementMigrationAggregationService.ImportPackageDocumentManagementPackageAsync(appId: appId, documentManagementPackage: documentManagementPackage);

    public DocumentManagementPackage ExportPackage(int appId, string packageName) =>
        documentManagementMigrationAggregationService.ExportPackage(appId: appId, packageName: packageName);
}