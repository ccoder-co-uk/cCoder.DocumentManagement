// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Services.Processings;
using FluentAssertions;
using Moq;
using Xunit;
using DmsPath = cCoder.DocumentManagement.Models.Path;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class DmsProcessingServiceTests
{
    [Fact]
    public async Task ShouldMoveAndReturnNoContentWhenPostIncludesMoveTo()
    {
        // Given
        DmsProcessingRequest request = CreateRequest(
            method: "POST",
            requestPath: "/api/dms/folder/file.txt",
            queryString: "?moveTo=folder/archive/file.txt",
            body: new MemoryStream([1, 2])
        );

        dmsInstanceServiceMock
            .Setup(expression: x =>
                x.MoveAsync(
                    It.Is<DmsPath>(path => path.FullPath == "folder/file.txt"),
                    It.Is<DmsPath>(path => path.FullPath == "folder/archive/file.txt")
                )
            )
            .Returns(value: ValueTask.CompletedTask);

        // When
        DmsProcessingResponse response = await dmsProcessingService.ProcessAsync(request: request);

        // Then
        response.StatusCode.Should().Be(expected: 204);
        response.HasBody.Should().BeFalse();
        dmsInstanceServiceMock.Verify(
            expression: x =>
                x.MoveAsync(
                    It.Is<DmsPath>(path => path.FullPath == "folder/file.txt"),
                    It.Is<DmsPath>(path => path.FullPath == "folder/archive/file.txt")
                ),
            times: Times.Once
        );
        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldReturnNoContentWithoutMoveWhenPostIncludesBlankMoveTo()
    {
        // Given
        DmsProcessingRequest request = CreateRequest(
            method: "POST",
            requestPath: "/api/dms/folder/file.txt",
            queryString: "?moveTo="
        );

        // When
        DmsProcessingResponse response = await dmsProcessingService.ProcessAsync(request: request);

        // Then
        response.StatusCode.Should().Be(expected: 204);
        response.HasBody.Should().BeFalse();
        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowInvalidOperationExceptionWhenPostUnpacksToFilePath()
    {
        // Given
        DmsProcessingRequest request = CreateRequest(
            method: "POST",
            requestPath: "/api/dms/folder/archive.zip",
            queryString: "?unpack=true",
            body: new MemoryStream([1, 2, 3])
        );

        // When
        Func<Task> act = async () => await dmsProcessingService.ProcessAsync(request: request);

        // Then
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage(expectedWildcardPattern: "Cannot unpack an archive to a file path");

        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldDelegateToUnpackWhenPostIncludesUnpack()
    {
        // Given
        byte[] originalBytes = [1, 2, 3, 4];
        MemoryStream requestBody = new(buffer: originalBytes);
        byte[] capturedBytes = [];
        DmsProcessingRequest request = CreateRequest(
            method: "POST",
            requestPath: "/api/dms/folder/archive",
            queryString: "?unpack=true&ignoreArchiveRoot=true",
            body: requestBody
        );

        dmsInstanceServiceMock
            .Setup(expression: x =>
                x.UnpackAsync(
                    It.Is<DmsPath>(path => path.FullPath == "folder/archive"),
                    It.IsAny<Stream>(),
                    true
                )
            )
            .Callback<DmsPath, Stream, bool>(action: (_, stream, _) => capturedBytes = ReadAllBytes(stream))
            .Returns(value: ValueTask.CompletedTask);

        // When
        DmsProcessingResponse response = await dmsProcessingService.ProcessAsync(request: request);

        // Then
        response.StatusCode.Should().Be(expected: 204);
        response.HasBody.Should().BeFalse();
        capturedBytes.Should().Equal(elements: originalBytes);
        dmsInstanceServiceMock.Verify(
            expression: x =>
                x.UnpackAsync(
                    It.Is<DmsPath>(path => path.FullPath == "folder/archive"),
                    It.IsAny<Stream>(),
                    true
                ),
            times: Times.Once
        );
        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldDelegateToSaveWhenPostDoesNotIncludeUnpack()
    {
        // Given
        byte[] originalBytes = [5, 6, 7];
        MemoryStream requestBody = new(buffer: originalBytes);
        byte[] capturedBytes = [];
        DmsProcessingRequest request = CreateRequest(
            method: "POST",
            requestPath: "/api/dms/folder/file.txt",
            queryString: string.Empty,
            body: requestBody
        );

        dmsInstanceServiceMock
            .Setup(expression: x =>
                x.SaveAsync(
                    It.Is<DmsPath>(path => path.FullPath == "folder/file.txt"),
                    It.IsAny<Stream>()
                )
            )
            .Callback<DmsPath, Stream>(action: (_, stream) => capturedBytes = ReadAllBytes(stream))
            .Returns(value: ValueTask.CompletedTask);

        // When
        DmsProcessingResponse response = await dmsProcessingService.ProcessAsync(request: request);

        // Then
        response.StatusCode.Should().Be(expected: 204);
        response.HasBody.Should().BeFalse();
        capturedBytes.Should().Equal(elements: originalBytes);
        dmsInstanceServiceMock.Verify(
            expression: x =>
                x.SaveAsync(
                    It.Is<DmsPath>(path => path.FullPath == "folder/file.txt"),
                    It.IsAny<Stream>()
                ),
            times: Times.Once
        );
        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }
}