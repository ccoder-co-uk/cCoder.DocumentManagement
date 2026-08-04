// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Brokers;
using cCoder.DocumentManagement.Exposures;
using cCoder.DocumentManagement.Models;
using cCoder.DocumentManagement.Services.Orchestrations;
using FluentAssertions;
using Moq;
using Xunit;
using DataFile = cCoder.Data.Models.DMS.File;
using DmsPath = cCoder.DocumentManagement.Dependencies.Path;


namespace cCoder.Core.Services.Tests.DMS.Brokers;

public partial class DmsInstanceFactoryTests
{
    [Fact]
    public void ShouldCreateDmsThatDelegatesSearch()
    {
        // Given
        IEnumerable<DataFile> expectedFiles = [new() { Id = Guid.NewGuid(), Name = "file.txt" }];
        var orchestrationServiceMock = new Mock<IDmsOrchestrationService>(behavior: MockBehavior.Strict);

        orchestrationServiceMock
            .Setup(expression: service =>
                service.SearchFilesDmsOperation(
                    operation: It.Is<DmsOperation>(match: operation =>
                        operation.Needle == "needle")))
            .Returns(value: new DmsOperation
            {
                Files = expectedFiles
            });

        var factory = new DmsInstanceFactory(dmsOrchestrationService: orchestrationServiceMock.Object);

        // When
        IDms dms = factory.CreateDms();
        IEnumerable<DataFile> actualFiles = dms.Search(needle: "needle");

        // Then
        actualFiles.Should()
            .BeSameAs(expected: expectedFiles);

        orchestrationServiceMock.Verify(
            expression: service =>
                service.SearchFilesDmsOperation(
                    operation: It.Is<DmsOperation>(match: operation =>
                        operation.Needle == "needle")),
            times: Times.Once);

        orchestrationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldCreateDmsThatDelegatesSaveAsync()
    {
        // Given
        var orchestrationServiceMock = new Mock<IDmsOrchestrationService>(behavior: MockBehavior.Strict);
        var path = new DmsPath(path: "content/file.txt");
        using var content = new MemoryStream(buffer: [1, 2, 3]);

        orchestrationServiceMock
            .Setup(expression: service =>
                service.SaveDmsOperationAsync(
                    operation: It.Is<DmsOperation>(match: operation =>
                        operation.Path == path.FullPath
                        && operation.Content == content)))
            .Returns(value: ValueTask.FromResult(
                result: new DmsOperation()));

        var factory = new DmsInstanceFactory(dmsOrchestrationService: orchestrationServiceMock.Object);

        IDms dms = factory.CreateDms();

        // When
        await dms.SaveAsync(path: path, content: content);

        // Then
        orchestrationServiceMock.Verify(
            expression: service =>
                service.SaveDmsOperationAsync(
                    operation: It.Is<DmsOperation>(match: operation =>
                        operation.Path == path.FullPath
                        && operation.Content == content)),
            times: Times.Once);

        orchestrationServiceMock.VerifyNoOtherCalls();
    }
}