// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Services.Processings;
using FluentAssertions;
using Moq;
using Xunit;
using DmsPath = cCoder.DocumentManagement.Dependencies.Path;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class DmsInstanceProcessingServiceTests
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
            .Setup(expression: x => x.DropAsync(path: It.Is<DmsPath>(match: path => path.FullPath == "folder/file.txt"), version: 4))
            .Returns(value: ValueTask.CompletedTask);

        // When
        DmsProcessingResponse response = await dmsProcessingService.ProcessDmsProcessingRequestAsync(request: request);

        // Then
        response.StatusCode.Should()
            .Be(expected: 204);

        response.ContentType.Should()
            .Be(expected: "application/json");

        response.HasBody.Should()
            .BeFalse();

        dmsInstanceServiceMock.Verify(
            expression: x => x.DropAsync(path: It.Is<DmsPath>(match: path => path.FullPath == "folder/file.txt"), version: 4),
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
            .Setup(expression: x => x.DropAsync(path: It.Is<DmsPath>(match: path => path.FullPath == "folder/file.txt"), version: 0))
            .Returns(value: ValueTask.CompletedTask);

        // When
        DmsProcessingResponse response = await dmsProcessingService.ProcessDmsProcessingRequestAsync(request: request);

        // Then
        response.StatusCode.Should()
            .Be(expected: 204);

        dmsInstanceServiceMock.Verify(
            expression: x => x.DropAsync(path: It.Is<DmsPath>(match: path => path.FullPath == "folder/file.txt"), version: 0),
            times: Times.Once
        );

        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }
}