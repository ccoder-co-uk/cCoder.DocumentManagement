// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.DocumentManagement.Services.Processings;
using FluentAssertions;
using Moq;
using Xunit;
using DmsPath = cCoder.DocumentManagement.Models.Path;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class WebDavProcessingServiceTests
{
    [Fact]
    public async Task ShouldReturnChallengeResponseWhenDmsInstanceThrowsSecurityException()
    {
        // Given
        DmsProcessingRequest request = CreateRequest(method: "DELETE", requestPath: "Core/App(7)/DAV/folder/file.txt");

        dmsInstanceServiceMock
            .Setup(expression: x => x.DropAsync(path: It.IsAny<DmsPath>(), version: 0))
            .Throws(exception: new SecurityException(message: "Denied"));

        // When
        DmsProcessingResponse response = await webDavProcessingService.ProcessDmsProcessingRequestAsync(request: request);

        // Then
        response.StatusCode.Should()
            .Be(expected: 204);

        response.Headers.Should()
            .Contain(predicate: header => header.Key == "WWW-Authenticate");

        dmsInstanceServiceMock.Verify(expression: x => x.DropAsync(path: It.IsAny<DmsPath>(), version: 0), times: Times.Once);
        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldReturnMessageBodyWhenMethodIsUnsupported()
    {
        // Given
        DmsProcessingRequest request = CreateRequest(method: "TRACE", requestPath: "Core/App(7)/DAV/folder/file.txt");

        // When
        DmsProcessingResponse response = await webDavProcessingService.ProcessDmsProcessingRequestAsync(request: request);
        string body = ReadBodyText(stream: response.Body);

        // Then
        response.StatusCode.Should()
            .Be(expected: 200);

        body.Should()
            .Contain(expected: "Unsupported WebDAV method: TRACE");

        dmsInstanceServiceMock.VerifyNoOtherCalls();
        fileServiceMock.VerifyNoOtherCalls();
        folderServiceMock.VerifyNoOtherCalls();
    }
}