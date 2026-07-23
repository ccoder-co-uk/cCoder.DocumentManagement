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
    public async Task ShouldReturnMultistatusWhenMethodIsPropPatch()
    {
        // Given
        DmsProcessingRequest request = CreateRequest(
            method: "PROPPATCH",
            requestPath: "Core/App(7)/DAV/folder/file.txt"
        );

        // When
        DmsProcessingResponse response = await webDavProcessingService.ProcessAsync(request: request);
        string xml = ReadBodyText(stream: response.Body);

        // Then
        response.StatusCode.Should()
            .Be(expected: 200);

        xml.Should()
            .Contain(expected: "multistatus");

        dmsInstanceServiceMock.VerifyNoOtherCalls();
        fileServiceMock.VerifyNoOtherCalls();
        folderServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldReturnOkWhenMethodIsLock()
    {
        // Given
        DmsProcessingRequest request = CreateRequest(method: "LOCK", requestPath: "Core/App(7)/DAV/folder/file.txt");

        // When
        DmsProcessingResponse response = await webDavProcessingService.ProcessAsync(request: request);

        // Then
        response.StatusCode.Should()
            .Be(expected: 200);

        response.HasBody.Should()
            .BeFalse();

        dmsInstanceServiceMock.VerifyNoOtherCalls();
        fileServiceMock.VerifyNoOtherCalls();
        folderServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldReturnNoContentWhenMethodIsUnlock()
    {
        // Given
        DmsProcessingRequest request = CreateRequest(method: "UNLOCK", requestPath: "Core/App(7)/DAV/folder/file.txt");

        // When
        DmsProcessingResponse response = await webDavProcessingService.ProcessAsync(request: request);

        // Then
        response.StatusCode.Should()
            .Be(expected: 204);

        response.HasBody.Should()
            .BeFalse();

        dmsInstanceServiceMock.VerifyNoOtherCalls();
        fileServiceMock.VerifyNoOtherCalls();
        folderServiceMock.VerifyNoOtherCalls();
    }
}