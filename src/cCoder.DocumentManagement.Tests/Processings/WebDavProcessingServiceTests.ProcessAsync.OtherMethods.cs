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
            "PROPPATCH",
            "Core/App(7)/DAV/folder/file.txt"
        );

        // When
        DmsProcessingResponse response = await webDavProcessingService.ProcessAsync(request);
        string xml = ReadBodyText(response.Body);

        // Then
        response.StatusCode.Should().Be(200);
        xml.Should().Contain("multistatus");
        dmsInstanceServiceMock.VerifyNoOtherCalls();
        fileServiceMock.VerifyNoOtherCalls();
        folderServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldReturnOkWhenMethodIsLock()
    {
        // Given
        DmsProcessingRequest request = CreateRequest("LOCK", "Core/App(7)/DAV/folder/file.txt");

        // When
        DmsProcessingResponse response = await webDavProcessingService.ProcessAsync(request);

        // Then
        response.StatusCode.Should().Be(200);
        response.HasBody.Should().BeFalse();
        dmsInstanceServiceMock.VerifyNoOtherCalls();
        fileServiceMock.VerifyNoOtherCalls();
        folderServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldReturnNoContentWhenMethodIsUnlock()
    {
        // Given
        DmsProcessingRequest request = CreateRequest("UNLOCK", "Core/App(7)/DAV/folder/file.txt");

        // When
        DmsProcessingResponse response = await webDavProcessingService.ProcessAsync(request);

        // Then
        response.StatusCode.Should().Be(204);
        response.HasBody.Should().BeFalse();
        dmsInstanceServiceMock.VerifyNoOtherCalls();
        fileServiceMock.VerifyNoOtherCalls();
        folderServiceMock.VerifyNoOtherCalls();
    }
}






