// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.DocumentManagement.Services.Processings;
using FluentAssertions;
using Moq;
using Xunit;
using DMSResult = cCoder.DocumentManagement.Models.DMSResult;
using DmsPath = cCoder.DocumentManagement.Models.Path;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class WebDavProcessingServiceTests
{
    [Fact]
    public async Task ShouldReturnDmsResultWhenMethodIsGet()
    {
        // Given
        MemoryStream stream = new(buffer: [1, 2, 3]);
        DMSResult result = new() { Data = stream, MimeType = "text/plain" };
        DmsProcessingRequest request = CreateRequest(
            method: "GET",
            requestPath: "Core/App(7)/DAV/folder/file.txt",
            queryString: "?version=3"
        );

        dmsInstanceServiceMock
            .Setup(expression: x => x.Get(It.Is<DmsPath>(path => path.FullPath == "folder/file.txt"), 3, ""))
            .Returns(value: result);

        // When
        DmsProcessingResponse response = await webDavProcessingService.ProcessAsync(request: request);

        // Then
        response.StatusCode.Should().Be(expected: 200);
        response.ContentType.Should().Be(expected: "text/plain");
        response.Body.Should().BeSameAs(expected: stream);
        dmsInstanceServiceMock.Verify(
            expression: x => x.Get(It.Is<DmsPath>(path => path.FullPath == "folder/file.txt"), 3, ""),
            times: Times.Once
        );
        dmsInstanceServiceMock.VerifyNoOtherCalls();
        fileServiceMock.VerifyNoOtherCalls();
        folderServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldDropPathWhenMethodIsDelete()
    {
        // Given
        DmsProcessingRequest request = CreateRequest(
            method: "DELETE",
            requestPath: "Core/App(7)/DAV/folder/file.txt",
            queryString: "?version=4"
        );

        dmsInstanceServiceMock
            .Setup(expression: x => x.DropAsync(It.Is<DmsPath>(path => path.FullPath == "folder/file.txt"), 4))
            .Returns(value: ValueTask.CompletedTask);

        // When
        DmsProcessingResponse response = await webDavProcessingService.ProcessAsync(request: request);

        // Then
        response.StatusCode.Should().Be(expected: 204);
        dmsInstanceServiceMock.Verify(
            expression: x => x.DropAsync(It.Is<DmsPath>(path => path.FullPath == "folder/file.txt"), 4),
            times: Times.Once
        );
        dmsInstanceServiceMock.VerifyNoOtherCalls();
        fileServiceMock.VerifyNoOtherCalls();
        folderServiceMock.VerifyNoOtherCalls();
    }
}