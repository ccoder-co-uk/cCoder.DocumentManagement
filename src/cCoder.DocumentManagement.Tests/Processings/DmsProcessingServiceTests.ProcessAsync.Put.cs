using cCoder.DocumentManagement.Services.Processings;
using FluentAssertions;
using Moq;
using Xunit;
using DmsPath = cCoder.DocumentManagement.Models.Path;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class DmsProcessingServiceTests
{
    [Fact]
    public async Task ShouldMoveAndReturnNoContentWhenPutIncludesMoveTo()
    {
        // Given
        DmsProcessingRequest request = CreateRequest(
            "PUT",
            "/api/dms/folder/file.txt",
            "?moveTo=folder/archive/file.txt",
            new MemoryStream([1, 2])
        );

        dmsInstanceServiceMock
            .Setup(x =>
                x.MoveAsync(
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
                x.MoveAsync(
                    It.Is<DmsPath>(path => path.FullPath == "folder/file.txt"),
                    It.Is<DmsPath>(path => path.FullPath == "folder/archive/file.txt")
                ),
            Times.Once
        );
        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldReturnNoContentWithoutMoveWhenPutIncludesBlankMoveTo()
    {
        // Given
        DmsProcessingRequest request = CreateRequest("PUT", "/api/dms/folder/file.txt", "?moveTo=");

        // When
        DmsProcessingResponse response = await dmsProcessingService.ProcessAsync(request);

        // Then
        response.StatusCode.Should().Be(204);
        response.HasBody.Should().BeFalse();
        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowInvalidOperationExceptionWhenPutUnpacksToFilePath()
    {
        // Given
        DmsProcessingRequest request = CreateRequest(
            "PUT",
            "/api/dms/folder/archive.zip",
            "?unpack=true",
            new MemoryStream([1, 2, 3])
        );

        // When
        Func<Task> act = async () => await dmsProcessingService.ProcessAsync(request);

        // Then
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Cannot unpack an archive to a file path");

        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldDelegateToUnpackWhenPutIncludesUnpack()
    {
        // Given
        byte[] originalBytes = [1, 2, 3, 4];
        MemoryStream requestBody = new(originalBytes);
        byte[] capturedBytes = [];
        DmsProcessingRequest request = CreateRequest(
            "PUT",
            "/api/dms/folder/archive",
            "?unpack=true&ignoreArchiveRoot=true",
            requestBody
        );

        dmsInstanceServiceMock
            .Setup(x =>
                x.UnpackAsync(
                    It.Is<DmsPath>(path => path.FullPath == "folder/archive"),
                    It.IsAny<Stream>(),
                    true
                )
            )
            .Callback<DmsPath, Stream, bool>((_, stream, _) => capturedBytes = ReadAllBytes(stream))
            .Returns(ValueTask.CompletedTask);

        // When
        DmsProcessingResponse response = await dmsProcessingService.ProcessAsync(request);

        // Then
        response.StatusCode.Should().Be(204);
        response.HasBody.Should().BeFalse();
        capturedBytes.Should().Equal(originalBytes);
        dmsInstanceServiceMock.Verify(
            x =>
                x.UnpackAsync(
                    It.Is<DmsPath>(path => path.FullPath == "folder/archive"),
                    It.IsAny<Stream>(),
                    true
                ),
            Times.Once
        );
        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldDelegateToSaveWhenPutDoesNotIncludeUnpack()
    {
        // Given
        byte[] originalBytes = [5, 6, 7];
        MemoryStream requestBody = new(originalBytes);
        byte[] capturedBytes = [];
        DmsProcessingRequest request = CreateRequest(
            "PUT",
            "/api/dms/folder/file.txt",
            string.Empty,
            requestBody
        );

        dmsInstanceServiceMock
            .Setup(x =>
                x.SaveAsync(
                    It.Is<DmsPath>(path => path.FullPath == "folder/file.txt"),
                    It.IsAny<Stream>()
                )
            )
            .Callback<DmsPath, Stream>((_, stream) => capturedBytes = ReadAllBytes(stream))
            .Returns(ValueTask.CompletedTask);

        // When
        DmsProcessingResponse response = await dmsProcessingService.ProcessAsync(request);

        // Then
        response.StatusCode.Should().Be(204);
        response.HasBody.Should().BeFalse();
        capturedBytes.Should().Equal(originalBytes);
        dmsInstanceServiceMock.Verify(
            x =>
                x.SaveAsync(
                    It.Is<DmsPath>(path => path.FullPath == "folder/file.txt"),
                    It.IsAny<Stream>()
                ),
            Times.Once
        );
        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }
}







