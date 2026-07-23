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
    public async Task ShouldCopyAndReturnNoContentWhenPostIncludesCopyTo()
    {
        // Given
        DmsProcessingRequest request = CreateRequest(
            method: "POST",
            requestPath: "/api/dms/folder/file.txt",
            queryString: "?copyTo=folder/archive/file.txt",
            body: new MemoryStream([1, 2])
        );

        dmsInstanceServiceMock
            .Setup(expression: x =>
                x.CopyAsync(
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
                x.CopyAsync(
                    It.Is<DmsPath>(path => path.FullPath == "folder/file.txt"),
                    It.Is<DmsPath>(path => path.FullPath == "folder/archive/file.txt")
                ),
            times: Times.Once
        );
        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldReturnNoContentWithoutCopyWhenPostIncludesBlankCopyTo()
    {
        // Given
        DmsProcessingRequest request = CreateRequest(
            method: "POST",
            requestPath: "/api/dms/folder/file.txt",
            queryString: "?copyTo="
        );

        // When
        DmsProcessingResponse response = await dmsProcessingService.ProcessAsync(request: request);

        // Then
        response.StatusCode.Should().Be(expected: 204);
        response.HasBody.Should().BeFalse();
        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldCopyAndReturnNoContentWhenPutIncludesCopyTo()
    {
        // Given
        DmsProcessingRequest request = CreateRequest(
            method: "PUT",
            requestPath: "/api/dms/folder/file.txt",
            queryString: "?copyTo=folder/archive/file.txt",
            body: new MemoryStream([1, 2])
        );

        dmsInstanceServiceMock
            .Setup(expression: x =>
                x.CopyAsync(
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
                x.CopyAsync(
                    It.Is<DmsPath>(path => path.FullPath == "folder/file.txt"),
                    It.Is<DmsPath>(path => path.FullPath == "folder/archive/file.txt")
                ),
            times: Times.Once
        );
        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldReturnNoContentWithoutCopyWhenPutIncludesBlankCopyTo()
    {
        // Given
        DmsProcessingRequest request = CreateRequest(method: "PUT", requestPath: "/api/dms/folder/file.txt", queryString: "?copyTo=");

        // When
        DmsProcessingResponse response = await dmsProcessingService.ProcessAsync(request: request);

        // Then
        response.StatusCode.Should().Be(expected: 204);
        response.HasBody.Should().BeFalse();
        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }
}