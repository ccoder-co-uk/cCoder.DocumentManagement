using cCoder.DocumentManagement.Services.Processings;
using FluentAssertions;
using Moq;
using Xunit;
using DmsPath = cCoder.DocumentManagement.Models.Path;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class WebDavProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToMoveWhenMethodIsMove()
    {
        // Given
        DmsProcessingRequest request = CreateRequest(
            "MOVE",
            "Core/App(7)/DAV/folder/file.txt",
            headers: new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
            {
                ["Authorization"] = ["Basic token"],
                ["Destination"] =
                [
                    "https://example.test/Api/Core/App(7)/DAV/folder/archive/file.txt",
                ],
            }
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
        DmsProcessingResponse response = await webDavProcessingService.ProcessAsync(request);

        // Then
        response.StatusCode.Should().Be(204);
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
    public async Task ShouldUseNormalizedDestinationPathWhenMethodIsCopy()
    {
        // Given
        DmsProcessingRequest request = CreateRequest(
            "COPY",
            "Core/App(7)/DAV/folder/file.txt",
            headers: new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
            {
                ["Authorization"] = ["Basic token"],
                ["Destination"] =
                [
                    "https://example.test/Api/Core/App(7)/DAV/folder/archive/file.txt",
                ],
            }
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
        DmsProcessingResponse response = await webDavProcessingService.ProcessAsync(request);

        // Then
        response.StatusCode.Should().Be(204);
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
}







