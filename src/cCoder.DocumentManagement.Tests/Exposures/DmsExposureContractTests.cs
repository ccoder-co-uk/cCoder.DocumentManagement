// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Exposures;
using cCoder.DocumentManagement.Services.Aggregations;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.DocumentManagement.Tests.Exposures;

public sealed partial class DmsExposureContractTests
{
    private readonly Mock<IDmsAggregationService> aggregationServiceMock = new(behavior: MockBehavior.Strict);
    private readonly Dms dms;

    public DmsExposureContractTests() =>
        dms = new Dms(dmsAggregationService: aggregationServiceMock.Object);

    [Fact]
    public void Get_WhenCalled_ReturnsAggregationResult()
    {
        // Given
        Models.DMSResult expected = new();

        aggregationServiceMock
            .Setup(expression: service => service.GetDmsOperation(
                dmsOperation: It.IsAny<DmsOperation>()))
            .Returns(value: new DmsOperation { Result = expected });

        // When
        Models.DMSResult actual = dms.Get(path: "file.txt");

        // Then
        actual.Should()
            .BeSameAs(expected: expected);

        aggregationServiceMock.VerifyAll();
    }

    [Fact]
    public void GetFilesZipped_WhenCalled_ReturnsAggregationResult()
    {
        // Given
        Models.DMSResult expected = new();

        aggregationServiceMock
            .Setup(expression: service => service.GetFilesZippedDmsOperation(
                dmsOperation: It.IsAny<DmsOperation>()))
            .Returns(value: new DmsOperation { Result = expected });

        // When
        Models.DMSResult actual = dms.GetFilesZipped(paths: ["file.txt"]);

        // Then
        actual.Should()
            .BeSameAs(expected: expected);

        aggregationServiceMock.VerifyAll();
    }

    [Fact]
    public async Task SaveAsync_WhenCalled_DelegatesToAggregationAsync()
    {
        // Given
        aggregationServiceMock
            .Setup(expression: service => service.SaveDmsOperationAsync(
                dmsOperation: It.IsAny<DmsOperation>()))
            .Returns(value: ValueTask.FromResult(result: new DmsOperation()));

        // When
        await dms.SaveAsync(path: "file.txt");

        // Then
        aggregationServiceMock.VerifyAll();
    }

    [Fact]
    public async Task UnpackAsync_WhenCalled_DelegatesToAggregationAsync()
    {
        // Given
        using Stream content = new MemoryStream();

        aggregationServiceMock
            .Setup(expression: service => service.UnpackDmsOperationAsync(
                dmsOperation: It.IsAny<DmsOperation>()))
            .Returns(value: ValueTask.FromResult(result: new DmsOperation()));

        // When
        await dms.UnpackAsync(path: "archive", content: content);

        // Then
        aggregationServiceMock.VerifyAll();
    }

    [Fact]
    public async Task DropAsync_WhenCalled_DelegatesToAggregationAsync()
    {
        // Given
        aggregationServiceMock
            .Setup(expression: service => service.DropDmsOperationAsync(
                dmsOperation: It.IsAny<DmsOperation>()))
            .Returns(value: ValueTask.FromResult(result: new DmsOperation()));

        // When
        await dms.DropAsync(path: "file.txt");

        // Then
        aggregationServiceMock.VerifyAll();
    }

    [Fact]
    public async Task CopyAsync_WhenCalled_DelegatesToAggregationAsync()
    {
        // Given
        aggregationServiceMock
            .Setup(expression: service => service.CopyDmsOperationAsync(
                dmsOperation: It.IsAny<DmsOperation>()))
            .Returns(value: ValueTask.FromResult(result: new DmsOperation()));

        // When
        await dms.CopyAsync(oldPath: "old", newPath: "new");

        // Then
        aggregationServiceMock.VerifyAll();
    }

    [Fact]
    public async Task MoveAsync_WhenCalled_DelegatesToAggregationAsync()
    {
        // Given
        aggregationServiceMock
            .Setup(expression: service => service.MoveDmsOperationAsync(
                dmsOperation: It.IsAny<DmsOperation>()))
            .Returns(value: ValueTask.FromResult(result: new DmsOperation()));

        // When
        await dms.MoveAsync(oldPath: "old", newPath: "new");

        // Then
        aggregationServiceMock.VerifyAll();
    }
}