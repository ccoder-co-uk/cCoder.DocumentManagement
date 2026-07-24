// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Services.Processings;
using FluentAssertions;
using Moq;
using Xunit;
using DmsPath = cCoder.DocumentManagement.Dependencies.Path;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class DmsProcessingServiceTests
{
    [Fact]
    public async Task ShouldMoveAndReturnNoContentWhenPutIncludesMoveTo()
    {
        // Given
        DmsProcessingRequest request = CreateRequest(
            method: "PUT",
            requestPath: "/api/dms/folder/file.txt",
            queryString: "?moveTo=folder/archive/file.txt",
            body: new MemoryStream(buffer: [1, 2])
        );

        dmsInstanceServiceMock
            .Setup(expression: x =>
                x.MoveAsync(
                    oldPath: It.Is<DmsPath>(match: path => path.FullPath == "folder/file.txt"),
                    newPath: It.Is<DmsPath>(match: path => path.FullPath == "folder/archive/file.txt")
                )
            )
            .Returns(value: ValueTask.CompletedTask);

        // When
        DmsProcessingResponse response = await dmsProcessingService.ProcessDmsProcessingRequestAsync(request: request);

        // Then
        response.StatusCode.Should()
            .Be(expected: 204);

        response.HasBody.Should()
            .BeFalse();

        dmsInstanceServiceMock.Verify(
            expression: x =>
                x.MoveAsync(
                    oldPath: It.Is<DmsPath>(match: path => path.FullPath == "folder/file.txt"),
                    newPath: It.Is<DmsPath>(match: path => path.FullPath == "folder/archive/file.txt")
                ),
            times: Times.Once
        );

        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldReturnNoContentWithoutMoveWhenPutIncludesBlankMoveTo()
    {
        // Given
        DmsProcessingRequest request = CreateRequest(method: "PUT", requestPath: "/api/dms/folder/file.txt", queryString: "?moveTo=");

        // When
        DmsProcessingResponse response = await dmsProcessingService.ProcessDmsProcessingRequestAsync(request: request);

        // Then
        response.StatusCode.Should()
            .Be(expected: 204);

        response.HasBody.Should()
            .BeFalse();

        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowInvalidOperationExceptionWhenPutUnpacksToFilePath()
    {
        // Given
        DmsProcessingRequest request = CreateRequest(
            method: "PUT",
            requestPath: "/api/dms/folder/archive.zip",
            queryString: "?unpack=true",
            body: new MemoryStream(buffer: [1, 2, 3])
        );

        // When
        Func<Task> act = async () => await dmsProcessingService.ProcessDmsProcessingRequestAsync(request: request);

        // Then
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage(expectedWildcardPattern: "Cannot unpack an archive to a file path");

        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldDelegateToUnpackWhenPutIncludesUnpack()
    {
        // Given
        byte[] originalBytes = [1, 2, 3, 4];
        MemoryStream requestBody = new(buffer: originalBytes);
        byte[] capturedBytes = [];

        DmsProcessingRequest request = CreateRequest(
            method: "PUT",
            requestPath: "/api/dms/folder/archive",
            queryString: "?unpack=true&ignoreArchiveRoot=true",
            body: requestBody
        );

        dmsInstanceServiceMock
            .Setup(expression: x =>
                x.UnpackAsync(
                    path: It.Is<DmsPath>(match: path => path.FullPath == "folder/archive"),
                    content: It.IsAny<Stream>(),
                    ignoreArchiveRoot: true
                )
            )
            .Callback<DmsPath, Stream, bool>(action: (_, stream, _) => capturedBytes = ReadAllBytes(stream: stream))
            .Returns(value: ValueTask.CompletedTask);

        // When
        DmsProcessingResponse response = await dmsProcessingService.ProcessDmsProcessingRequestAsync(request: request);

        // Then
        response.StatusCode.Should()
            .Be(expected: 204);

        response.HasBody.Should()
            .BeFalse();

        capturedBytes.Should()
            .Equal(elements: originalBytes);

        dmsInstanceServiceMock.Verify(
            expression: x =>
                x.UnpackAsync(
                    path: It.Is<DmsPath>(match: path => path.FullPath == "folder/archive"),
                    content: It.IsAny<Stream>(),
                    ignoreArchiveRoot: true
                ),
            times: Times.Once
        );

        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldDelegateToSaveWhenPutDoesNotIncludeUnpack()
    {
        // Given
        byte[] originalBytes = [5, 6, 7];
        MemoryStream requestBody = new(buffer: originalBytes);
        byte[] capturedBytes = [];

        DmsProcessingRequest request = CreateRequest(
            method: "PUT",
            requestPath: "/api/dms/folder/file.txt",
            queryString: string.Empty,
            body: requestBody
        );

        dmsInstanceServiceMock
            .Setup(expression: x =>
                x.SaveAsync(
                    path: It.Is<DmsPath>(match: path => path.FullPath == "folder/file.txt"),
                    content: It.IsAny<Stream>()
                )
            )
            .Callback<DmsPath, Stream>(action: (_, stream) => capturedBytes = ReadAllBytes(stream: stream))
            .Returns(value: ValueTask.CompletedTask);

        // When
        DmsProcessingResponse response = await dmsProcessingService.ProcessDmsProcessingRequestAsync(request: request);

        // Then
        response.StatusCode.Should()
            .Be(expected: 204);

        response.HasBody.Should()
            .BeFalse();

        capturedBytes.Should()
            .Equal(elements: originalBytes);

        dmsInstanceServiceMock.Verify(
            expression: x =>
                x.SaveAsync(
                    path: It.Is<DmsPath>(match: path => path.FullPath == "folder/file.txt"),
                    content: It.IsAny<Stream>()
                ),
            times: Times.Once
        );

        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }
}