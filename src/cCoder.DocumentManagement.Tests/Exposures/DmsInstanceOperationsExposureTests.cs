// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Exposures;
using cCoder.DocumentManagement.Services.Aggregations;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.DocumentManagement.Tests.Exposures;

public sealed partial class DmsInstanceOperationsExposureTests
{
    private readonly Mock<IDmsAggregationService> aggregationServiceMock = new(behavior: MockBehavior.Strict);
    private readonly DmsInstanceOperationsExposure exposure;

    public DmsInstanceOperationsExposureTests() =>
        exposure = new DmsInstanceOperationsExposure(dmsAggregationService: aggregationServiceMock.Object);

    [Fact]
    public void Get_WhenCalled_MapsOperationAndReturnsResult()
    {
        // Given
        Models.DMSResult expected = new();

        aggregationServiceMock
            .Setup(expression: service => service.GetDmsOperation(
                dmsOperation: It.Is<DmsOperation>(match: operation => operation.Path == "folder/file.txt"
                    && operation.Version == 3
                    && operation.Search == "needle")))
            .Returns(value: new DmsOperation { Result = expected });

        // When
        Models.DMSResult actual = exposure.Get(path: "folder/file.txt", version: 3, search: "needle");

        // Then
        actual.Should()
            .BeSameAs(expected: expected);

        aggregationServiceMock.VerifyAll();
    }

    [Fact]
    public void GetFilesZipped_WhenCalled_MapsPathsAndReturnsResult()
    {
        // Given
        string[] paths = ["one", "two"];
        Models.DMSResult expected = new();

        aggregationServiceMock
            .Setup(expression: service => service.GetFilesZippedDmsOperation(
                dmsOperation: It.Is<DmsOperation>(match: operation => operation.Paths == paths)))
            .Returns(value: new DmsOperation { Result = expected });

        // When
        Models.DMSResult actual = exposure.GetFilesZipped(paths: paths);

        // Then
        actual.Should()
            .BeSameAs(expected: expected);

        aggregationServiceMock.VerifyAll();
    }

    [Fact]
    public async Task SaveAsync_WhenCalled_MapsOperationAsync()
    {
        // Given
        using Stream content = new MemoryStream(buffer: [1]);

        aggregationServiceMock
            .Setup(expression: service => service.SaveDmsOperationAsync(
                dmsOperation: It.Is<DmsOperation>(match: operation =>
                    operation.Path == "file.txt" && operation.Content == content)))
            .Returns(value: ValueTask.FromResult(result: new DmsOperation()));

        // When
        await exposure.SaveAsync(path: "file.txt", content: content);

        // Then
        aggregationServiceMock.VerifyAll();
    }

    [Fact]
    public async Task UnpackAsync_WhenCalled_MapsOperationAsync()
    {
        // Given
        using Stream content = new MemoryStream(buffer: [1]);

        aggregationServiceMock
            .Setup(expression: service => service.UnpackDmsOperationAsync(
                dmsOperation: It.Is<DmsOperation>(match: operation => operation.Path == "archive"
                    && operation.Content == content
                    && operation.IgnoreArchiveRoot)))
            .Returns(value: ValueTask.FromResult(result: new DmsOperation()));

        // When
        await exposure.UnpackAsync(path: "archive", content: content, ignoreArchiveRoot: true);

        // Then
        aggregationServiceMock.VerifyAll();
    }

    [Fact]
    public async Task DropAsync_WhenCalled_MapsOperationAsync()
    {
        // Given
        aggregationServiceMock
            .Setup(expression: service => service.DropDmsOperationAsync(
                dmsOperation: It.Is<DmsOperation>(match: operation =>
                    operation.Path == "file.txt" && operation.Version == 2)))
            .Returns(value: ValueTask.FromResult(result: new DmsOperation()));

        // When
        await exposure.DropAsync(path: "file.txt", version: 2);

        // Then
        aggregationServiceMock.VerifyAll();
    }

    [Fact]
    public async Task CopyAsync_WhenCalled_MapsOperationAsync()
    {
        // Given
        aggregationServiceMock
            .Setup(expression: service => service.CopyDmsOperationAsync(
                dmsOperation: It.Is<DmsOperation>(match: operation =>
                    operation.Path == "old" && operation.NewPath == "new")))
            .Returns(value: ValueTask.FromResult(result: new DmsOperation()));

        // When
        await exposure.CopyAsync(oldPath: "old", newPath: "new");

        // Then
        aggregationServiceMock.VerifyAll();
    }

    [Fact]
    public async Task MoveAsync_WhenCalled_MapsOperationAsync()
    {
        // Given
        aggregationServiceMock
            .Setup(expression: service => service.MoveDmsOperationAsync(
                dmsOperation: It.Is<DmsOperation>(match: operation =>
                    operation.Path == "old" && operation.NewPath == "new")))
            .Returns(value: ValueTask.FromResult(result: new DmsOperation()));

        // When
        await exposure.MoveAsync(oldPath: "old", newPath: "new");

        // Then
        aggregationServiceMock.VerifyAll();
    }
}