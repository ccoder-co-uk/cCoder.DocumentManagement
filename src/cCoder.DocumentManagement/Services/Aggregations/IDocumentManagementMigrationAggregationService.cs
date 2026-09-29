// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;

using cCoder.DocumentManagement.Models;


namespace cCoder.DocumentManagement.Services.Aggregations;

internal interface IDocumentManagementMigrationAggregationService
{
    ValueTask ImportPackageDocumentManagementPackageAsync(int appId, DocumentManagementPackage documentManagementPackage);

    DocumentManagementPackage ExportPackage(int appId, string packageName);
}