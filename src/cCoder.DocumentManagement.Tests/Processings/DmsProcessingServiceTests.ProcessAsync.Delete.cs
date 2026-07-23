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
    public async Task ShouldDropAndReturnNoContentWhenMethodIsDelete()
    {
        // Given
        DmsProcessingRequest request = CreateRequest(
            method: "DELETE",
            requestPath: "/api/dms/folder/file.txt",
            queryString: "?version=4"
        );

        dmsInstanceServiceMock
            .Setup(expression: x => x.DropAsync(It.Is<DmsPath>(path => path.FullPath == "folder/file.txt"), 4))
            .Returns(value: ValueTask.CompletedTask);

        // When
        DmsProcessingResponse response = await dmsProcessingService.ProcessAsync(request: request);

        // Then
        response.StatusCode.Should().Be(expected: 204);
        response.ContentType.Should().Be(expected: "application/json");
        response.HasBody.Should().BeFalse();
        dmsInstanceServiceMock.Verify(
            expression: x => x.DropAsync(It.Is<DmsPath>(path => path.FullPath == "folder/file.txt"), 4),
            times: Times.Once
        );
        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldUseZeroWhenDeleteVersionIsMissing()
    {
        // Given
        DmsProcessingRequest request = CreateRequest(method: "DELETE", requestPath: "/api/dms/folder/file.txt");

        dmsInstanceServiceMock
            .Setup(expression: x => x.DropAsync(It.Is<DmsPath>(path => path.FullPath == "folder/file.txt"), 0))
            .Returns(value: ValueTask.CompletedTask);

        // When
        DmsProcessingResponse response = await dmsProcessingService.ProcessAsync(request: request);

        // Then
        response.StatusCode.Should().Be(expected: 204);
        dmsInstanceServiceMock.Verify(
            expression: x => x.DropAsync(It.Is<DmsPath>(path => path.FullPath == "folder/file.txt"), 0),
            times: Times.Once
        );
        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }
}