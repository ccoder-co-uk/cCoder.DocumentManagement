// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Services.Processings;
using FluentAssertions;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class WebDavProcessingServiceTests
{
    [Fact]
    public async Task ShouldReturnUnauthorizedResponseWhenAuthorizationHeaderIsMissing()
    {
        // Given
        DmsProcessingRequest request = CreateRequest(
            method: "GET",
            headers: new Dictionary<string, string[]>(comparer: StringComparer.OrdinalIgnoreCase)
        );

        // When
        DmsProcessingResponse response = await webDavProcessingService.ProcessAsync(request: request);

        // Then
        response.StatusCode.Should().Be(expected: 401);
        response.ContentType.Should().Be(expected: "text/xml; charset=\"utf-8\"");
        response.HasBody.Should().BeTrue();
        response.Headers.Should().Contain(predicate: header => header.Key == "WWW-Authenticate");
        dmsInstanceServiceMock.VerifyNoOtherCalls();
        fileServiceMock.VerifyNoOtherCalls();
        folderServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldReturnDavHeadersWhenMethodIsOptions()
    {
        // Given
        DmsProcessingRequest request = CreateRequest(method: "OPTIONS", requestPath: "Core/App(7)/DAV/folder");

        // When
        DmsProcessingResponse response = await webDavProcessingService.ProcessAsync(request: request);

        // Then
        response.StatusCode.Should().Be(expected: 204);
        response.HasBody.Should().BeFalse();
        response.Headers.Should().Contain(predicate: header => header.Key == "DAV" && header.Value == "1, 2");
        dmsInstanceServiceMock.VerifyNoOtherCalls();
        fileServiceMock.VerifyNoOtherCalls();
        folderServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldReturnNoContentWhenMethodIsHead()
    {
        // Given
        DmsProcessingRequest request = CreateRequest(method: "HEAD", requestPath: "Core/App(7)/DAV/folder/file.txt");

        // When
        DmsProcessingResponse response = await webDavProcessingService.ProcessAsync(request: request);

        // Then
        response.StatusCode.Should().Be(expected: 204);
        response.HasBody.Should().BeFalse();
        dmsInstanceServiceMock.VerifyNoOtherCalls();
        fileServiceMock.VerifyNoOtherCalls();
        folderServiceMock.VerifyNoOtherCalls();
    }
}