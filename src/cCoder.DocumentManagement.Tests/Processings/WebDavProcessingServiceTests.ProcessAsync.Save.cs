using cCoder.DocumentManagement.Services.Processings;
using FluentAssertions;
using Moq;
using Xunit;
using DmsPath = cCoder.DocumentManagement.Models.Path;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class WebDavProcessingServiceTests
{
    [Fact]
    public async Task ShouldSaveAndReturnCreatedResponseWhenMethodIsPost()
    {
        // Given
        byte[] bytes = [1, 2, 3];
        MemoryStream body = new(bytes);
        byte[] capturedBytes = [];
        DmsProcessingRequest request = CreateRequest(
            "POST",
            "Core/App(7)/DAV/folder/file.txt",
            body: body
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
        DmsProcessingResponse response = await webDavProcessingService.ProcessAsync(request);

        // Then
        response.StatusCode.Should().Be(201);
        response.ContentType.Should().Be("text/plain");
        capturedBytes.Should().Equal(bytes);
        dmsInstanceServiceMock.Verify(
            x =>
                x.SaveAsync(
                    It.Is<DmsPath>(path => path.FullPath == "folder/file.txt"),
                    It.IsAny<Stream>()
                ),
            Times.Once
        );
        dmsInstanceServiceMock.VerifyNoOtherCalls();
        fileServiceMock.VerifyNoOtherCalls();
        folderServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldSaveAndReturnCreatedResponseWhenMethodIsPut()
    {
        // Given
        byte[] bytes = [4, 5, 6];
        MemoryStream body = new(bytes);
        byte[] capturedBytes = [];
        DmsProcessingRequest request = CreateRequest(
            "PUT",
            "Core/App(7)/DAV/folder/file.txt",
            body: body
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
        DmsProcessingResponse response = await webDavProcessingService.ProcessAsync(request);

        // Then
        response.StatusCode.Should().Be(201);
        capturedBytes.Should().Equal(bytes);
        dmsInstanceServiceMock.Verify(
            x =>
                x.SaveAsync(
                    It.Is<DmsPath>(path => path.FullPath == "folder/file.txt"),
                    It.IsAny<Stream>()
                ),
            Times.Once
        );
        dmsInstanceServiceMock.VerifyNoOtherCalls();
        fileServiceMock.VerifyNoOtherCalls();
        folderServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldSaveAndReturnNoContentWhenMethodIsMkCol()
    {
        // Given
        DmsProcessingRequest request = CreateRequest("MKCOL", "Core/App(7)/DAV/folder");

        dmsInstanceServiceMock
            .Setup(x =>
                x.SaveAsync(It.Is<DmsPath>(path => path.FullPath == "folder"), It.IsAny<Stream>())
            )
            .Returns(ValueTask.CompletedTask);

        // When
        DmsProcessingResponse response = await webDavProcessingService.ProcessAsync(request);

        // Then
        response.StatusCode.Should().Be(204);
        dmsInstanceServiceMock.Verify(
            x => x.SaveAsync(It.Is<DmsPath>(path => path.FullPath == "folder"), It.IsAny<Stream>()),
            Times.Once
        );
        dmsInstanceServiceMock.VerifyNoOtherCalls();
        fileServiceMock.VerifyNoOtherCalls();
        folderServiceMock.VerifyNoOtherCalls();
    }
}







