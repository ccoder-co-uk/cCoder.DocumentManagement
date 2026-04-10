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
        MemoryStream stream = new([1, 2, 3]);
        DMSResult result = new() { Data = stream, MimeType = "text/plain" };
        DmsProcessingRequest request = CreateRequest(
            "GET",
            "Core/App(7)/DAV/folder/file.txt",
            "?version=3"
        );

        dmsInstanceServiceMock
            .Setup(x => x.Get(It.Is<DmsPath>(path => path.FullPath == "folder/file.txt"), 3, ""))
            .Returns(result);

        // When
        DmsProcessingResponse response = await webDavProcessingService.ProcessAsync(request);

        // Then
        response.StatusCode.Should().Be(200);
        response.ContentType.Should().Be("text/plain");
        response.Body.Should().BeSameAs(stream);
        dmsInstanceServiceMock.Verify(
            x => x.Get(It.Is<DmsPath>(path => path.FullPath == "folder/file.txt"), 3, ""),
            Times.Once
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
            "DELETE",
            "Core/App(7)/DAV/folder/file.txt",
            "?version=4"
        );

        dmsInstanceServiceMock
            .Setup(x => x.DropAsync(It.Is<DmsPath>(path => path.FullPath == "folder/file.txt"), 4))
            .Returns(ValueTask.CompletedTask);

        // When
        DmsProcessingResponse response = await webDavProcessingService.ProcessAsync(request);

        // Then
        response.StatusCode.Should().Be(204);
        dmsInstanceServiceMock.Verify(
            x => x.DropAsync(It.Is<DmsPath>(path => path.FullPath == "folder/file.txt"), 4),
            Times.Once
        );
        dmsInstanceServiceMock.VerifyNoOtherCalls();
        fileServiceMock.VerifyNoOtherCalls();
        folderServiceMock.VerifyNoOtherCalls();
    }
}







