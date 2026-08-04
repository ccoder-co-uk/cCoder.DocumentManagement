// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Exposures;
using cCoder.DocumentManagement.Models;
using cCoder.DocumentManagement.Services.Aggregations;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.DMS.Exposures;

public partial class DocumentManagementPackageManagerTests
{
    [Fact]
    public async Task ShouldDelegatePackageImportAndExportAsync()
    {
        // Given
        const int appId = 42;
        const string packageName = "documents";
        DocumentManagementPackage package = new();
        DocumentManagementPackage exportedPackage = new();

        Mock<IDocumentManagementMigrationAggregationService> serviceMock =
            new(behavior: MockBehavior.Strict);

        serviceMock
            .Setup(expression: service => service.ImportPackageDocumentManagementPackageAsync(
                appId: appId,
                package: package))
            .Returns(value: ValueTask.CompletedTask);

        serviceMock
            .Setup(expression: service => service.ExportPackage(
                appId: appId,
                packageName: packageName))
            .Returns(value: exportedPackage);

        DocumentManagementPackageManager manager = new(
            documentManagementMigrationAggregationService: serviceMock.Object);

        // When
        await manager.ImportPackageAsync(
            appId: appId,
            package: package);

        DocumentManagementPackage result = manager.ExportPackage(
            appId: appId,
            packageName: packageName);

        // Then
        result.Should()
            .BeSameAs(expected: exportedPackage);

        serviceMock.VerifyAll();
    }
}