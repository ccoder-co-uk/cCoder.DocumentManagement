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
        DmsProcessingRequest request = CreateRequest("DELETE", "Core/App(7)/DAV/folder/file.txt");

        dmsInstanceServiceMock
            .Setup(x => x.DropAsync(It.IsAny<DmsPath>(), 0))
            .Throws(new SecurityException("Denied"));

        // When
        DmsProcessingResponse response = await webDavProcessingService.ProcessAsync(request);

        // Then
        response.StatusCode.Should().Be(204);
        response.Headers.Should().Contain(header => header.Key == "WWW-Authenticate");
        dmsInstanceServiceMock.Verify(x => x.DropAsync(It.IsAny<DmsPath>(), 0), Times.Once);
        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldReturnMessageBodyWhenMethodIsUnsupported()
    {
        // Given
        DmsProcessingRequest request = CreateRequest("TRACE", "Core/App(7)/DAV/folder/file.txt");

        // When
        DmsProcessingResponse response = await webDavProcessingService.ProcessAsync(request);
        string body = ReadBodyText(response.Body);

        // Then
        response.StatusCode.Should().Be(200);
        body.Should().Contain("Unsupported WebDAV method: TRACE");
        dmsInstanceServiceMock.VerifyNoOtherCalls();
        fileServiceMock.VerifyNoOtherCalls();
        folderServiceMock.VerifyNoOtherCalls();
    }
}







