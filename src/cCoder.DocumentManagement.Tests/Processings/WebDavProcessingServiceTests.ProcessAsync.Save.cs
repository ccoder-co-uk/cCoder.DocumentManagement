// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Services.Processings;
using FluentAssertions;
using Moq;
using Xunit;
using DmsPath = cCoder.DocumentManagement.Models.Path;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class WebDavProcessingServiceTests
{
    [Fact]
    public async Task ShouldSaveAndReturnCreatedResponseWhenMethodIsPost()
    {
        // Given
        byte[] bytes = [1, 2, 3];
        MemoryStream body = new(buffer: bytes);
        byte[] capturedBytes = [];

        DmsProcessingRequest request = CreateRequest(
            method: "POST",
            requestPath: "Core/App(7)/DAV/folder/file.txt",
            body: body
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
        DmsProcessingResponse response = await webDavProcessingService.ProcessAsync(request: request);

        // Then
        response.StatusCode.Should()
            .Be(expected: 201);

        response.ContentType.Should()
            .Be(expected: "text/plain");

        capturedBytes.Should()
            .Equal(elements: bytes);

        dmsInstanceServiceMock.Verify(
            expression: x =>
                x.SaveAsync(
                    path: It.Is<DmsPath>(match: path => path.FullPath == "folder/file.txt"),
                    content: It.IsAny<Stream>()
                ),
            times: Times.Once
        );

        dmsInstanceServiceMock.VerifyNoOtherCalls();
        fileServiceMock.VerifyNoOtherCalls();
        folderServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldSaveAndReturnCreatedResponseWhenMethodIsPut()
    {
        // Given
        byte[] bytes = [4, 5, 6];
        MemoryStream body = new(buffer: bytes);
        byte[] capturedBytes = [];

        DmsProcessingRequest request = CreateRequest(
            method: "PUT",
            requestPath: "Core/App(7)/DAV/folder/file.txt",
            body: body
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
        DmsProcessingResponse response = await webDavProcessingService.ProcessAsync(request: request);

        // Then
        response.StatusCode.Should()
            .Be(expected: 201);

        capturedBytes.Should()
            .Equal(elements: bytes);

        dmsInstanceServiceMock.Verify(
            expression: x =>
                x.SaveAsync(
                    path: It.Is<DmsPath>(match: path => path.FullPath == "folder/file.txt"),
                    content: It.IsAny<Stream>()
                ),
            times: Times.Once
        );

        dmsInstanceServiceMock.VerifyNoOtherCalls();
        fileServiceMock.VerifyNoOtherCalls();
        folderServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldSaveAndReturnNoContentWhenMethodIsMkCol()
    {
        // Given
        DmsProcessingRequest request = CreateRequest(method: "MKCOL", requestPath: "Core/App(7)/DAV/folder");

        dmsInstanceServiceMock
            .Setup(expression: x =>
                x.SaveAsync(path: It.Is<DmsPath>(match: path => path.FullPath == "folder"), content: It.IsAny<Stream>())
            )
            .Returns(value: ValueTask.CompletedTask);

        // When
        DmsProcessingResponse response = await webDavProcessingService.ProcessAsync(request: request);

        // Then
        response.StatusCode.Should()
            .Be(expected: 204);

        dmsInstanceServiceMock.Verify(
            expression: x => x.SaveAsync(path: It.Is<DmsPath>(match: path => path.FullPath == "folder"), content: It.IsAny<Stream>()),
            times: Times.Once
        );

        dmsInstanceServiceMock.VerifyNoOtherCalls();
        fileServiceMock.VerifyNoOtherCalls();
        folderServiceMock.VerifyNoOtherCalls();
    }
}