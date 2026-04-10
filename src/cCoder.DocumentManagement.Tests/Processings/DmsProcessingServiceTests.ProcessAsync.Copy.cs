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
            "POST",
            "/api/dms/folder/file.txt",
            "?copyTo=folder/archive/file.txt",
            new MemoryStream([1, 2])
        );

        dmsInstanceServiceMock
            .Setup(x =>
                x.CopyAsync(
                    It.Is<DmsPath>(path => path.FullPath == "folder/file.txt"),
                    It.Is<DmsPath>(path => path.FullPath == "folder/archive/file.txt")
                )
            )
            .Returns(ValueTask.CompletedTask);

        // When
        DmsProcessingResponse response = await dmsProcessingService.ProcessAsync(request);

        // Then
        response.StatusCode.Should().Be(204);
        response.HasBody.Should().BeFalse();
        dmsInstanceServiceMock.Verify(
            x =>
                x.CopyAsync(
                    It.Is<DmsPath>(path => path.FullPath == "folder/file.txt"),
                    It.Is<DmsPath>(path => path.FullPath == "folder/archive/file.txt")
                ),
            Times.Once
        );
        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldReturnNoContentWithoutCopyWhenPostIncludesBlankCopyTo()
    {
        // Given
        DmsProcessingRequest request = CreateRequest(
            "POST",
            "/api/dms/folder/file.txt",
            "?copyTo="
        );

        // When
        DmsProcessingResponse response = await dmsProcessingService.ProcessAsync(request);

        // Then
        response.StatusCode.Should().Be(204);
        response.HasBody.Should().BeFalse();
        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldCopyAndReturnNoContentWhenPutIncludesCopyTo()
    {
        // Given
        DmsProcessingRequest request = CreateRequest(
            "PUT",
            "/api/dms/folder/file.txt",
            "?copyTo=folder/archive/file.txt",
            new MemoryStream([1, 2])
        );

        dmsInstanceServiceMock
            .Setup(x =>
                x.CopyAsync(
                    It.Is<DmsPath>(path => path.FullPath == "folder/file.txt"),
                    It.Is<DmsPath>(path => path.FullPath == "folder/archive/file.txt")
                )
            )
            .Returns(ValueTask.CompletedTask);

        // When
        DmsProcessingResponse response = await dmsProcessingService.ProcessAsync(request);

        // Then
        response.StatusCode.Should().Be(204);
        response.HasBody.Should().BeFalse();
        dmsInstanceServiceMock.Verify(
            x =>
                x.CopyAsync(
                    It.Is<DmsPath>(path => path.FullPath == "folder/file.txt"),
                    It.Is<DmsPath>(path => path.FullPath == "folder/archive/file.txt")
                ),
            Times.Once
        );
        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldReturnNoContentWithoutCopyWhenPutIncludesBlankCopyTo()
    {
        // Given
        DmsProcessingRequest request = CreateRequest("PUT", "/api/dms/folder/file.txt", "?copyTo=");

        // When
        DmsProcessingResponse response = await dmsProcessingService.ProcessAsync(request);

        // Then
        response.StatusCode.Should().Be(204);
        response.HasBody.Should().BeFalse();
        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }
}







