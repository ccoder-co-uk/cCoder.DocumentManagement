using cCoder.Data;
using cCoder.DocumentManagement.Services.Processings;
using FluentAssertions;
using Moq;
using Xunit;
using DMSResult = cCoder.DocumentManagement.Models.DMSResult;
using DmsPath = cCoder.DocumentManagement.Models.Path;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class DmsProcessingServiceTests
{
    [Fact]
    public async Task ShouldReturnBodyAndMimeTypeWhenGetReturnsResult()
    {
        // Given
        MemoryStream body = new([1, 2, 3]);
        DMSResult result = new() { Data = body, MimeType = "text/plain" };
        DmsProcessingRequest request = CreateRequest(
            "GET",
            "/api/dms/folder/file.txt",
            "?version=3&search=needle"
        );

        dmsInstanceServiceMock
            .Setup(x =>
                x.Get(It.Is<DmsPath>(path => path.FullPath == "folder/file.txt"), 3, "needle")
            )
            .Returns(result);

        // When
        DmsProcessingResponse response = await dmsProcessingService.ProcessAsync(request);

        // Then
        response.StatusCode.Should().Be(200);
        response.ContentType.Should().Be("text/plain");
        response.HasBody.Should().BeTrue();
        response.Body.Should().BeSameAs(body);
        dmsInstanceServiceMock.Verify(
            x => x.Get(It.Is<DmsPath>(path => path.FullPath == "folder/file.txt"), 3, "needle"),
            Times.Once
        );
        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldZipAndReturnOctetStreamWhenGetIncludesDownloadPaths()
    {
        // Given
        string[] expectedPaths = ["one.txt", "two.txt"];
        MemoryStream body = new([9, 8, 7]);
        DMSResult result = new() { Data = body, MimeType = "application/zip" };
        DmsProcessingRequest request = CreateRequest(
            "GET",
            "/api/dms/folder",
            "?download=true&downloadPaths=one.txt,two.txt"
        );

        dmsInstanceServiceMock
            .Setup(x =>
                x.GetFilesZipped(
                    It.Is<IEnumerable<DmsPath>>(paths =>
                        paths.Select(path => path.FullPath).SequenceEqual(expectedPaths)
                    )
                )
            )
            .Returns(result);

        // When
        DmsProcessingResponse response = await dmsProcessingService.ProcessAsync(request);

        // Then
        response.StatusCode.Should().Be(200);
        response.ContentType.Should().Be("application/octet-stream");
        response.HasBody.Should().BeTrue();
        response.Body.Should().BeSameAs(body);
        dmsInstanceServiceMock.Verify(
            x =>
                x.GetFilesZipped(
                    It.Is<IEnumerable<DmsPath>>(paths =>
                        paths.Select(path => path.FullPath).SequenceEqual(expectedPaths)
                    )
                ),
            Times.Once
        );
        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldReturnNoContentWhenGetReturnsNull()
    {
        // Given
        DmsProcessingRequest request = CreateRequest("GET", "/api/dms/folder/file.txt");

        dmsInstanceServiceMock
            .Setup(x =>
                x.Get(It.Is<DmsPath>(path => path.FullPath == "folder/file.txt"), 0, string.Empty)
            )
            .Returns((DMSResult)null!);

        // When
        DmsProcessingResponse response = await dmsProcessingService.ProcessAsync(request);

        // Then
        response.StatusCode.Should().Be(204);
        response.ContentType.Should().Be("plain/text");
        response.HasBody.Should().BeFalse();
        dmsInstanceServiceMock.Verify(
            x => x.Get(It.Is<DmsPath>(path => path.FullPath == "folder/file.txt"), 0, string.Empty),
            Times.Once
        );
        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }
}






