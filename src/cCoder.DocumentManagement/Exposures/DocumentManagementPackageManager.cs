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
        documentManagementMigrationAggregationService.ImportPackageAsync(appId, package);

    public DocumentManagementPackage ExportPackage(int appId, string packageName) =>
        documentManagementMigrationAggregationService.ExportPackage(appId, packageName);
}


