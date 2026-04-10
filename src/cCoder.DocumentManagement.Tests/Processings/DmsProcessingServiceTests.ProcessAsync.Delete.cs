using cCoder.DocumentManagement.Services.Processings;
using FluentAssertions;
using Moq;
using Xunit;
using DmsPath = cCoder.DocumentManagement.Models.Path;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class DmsProcessingServiceTests
{
    [Fact]
    public async Task ShouldDropAndReturnNoContentWhenMethodIsDelete()
    {
        // Given
        DmsProcessingRequest request = CreateRequest(
            "DELETE",
            "/api/dms/folder/file.txt",
            "?version=4"
        );

        dmsInstanceServiceMock
            .Setup(x => x.DropAsync(It.Is<DmsPath>(path => path.FullPath == "folder/file.txt"), 4))
            .Returns(ValueTask.CompletedTask);

        // When
        DmsProcessingResponse response = await dmsProcessingService.ProcessAsync(request);

        // Then
        response.StatusCode.Should().Be(204);
        response.ContentType.Should().Be("application/json");
        response.HasBody.Should().BeFalse();
        dmsInstanceServiceMock.Verify(
            x => x.DropAsync(It.Is<DmsPath>(path => path.FullPath == "folder/file.txt"), 4),
            Times.Once
        );
        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldUseZeroWhenDeleteVersionIsMissing()
    {
        // Given
        DmsProcessingRequest request = CreateRequest("DELETE", "/api/dms/folder/file.txt");

        dmsInstanceServiceMock
            .Setup(x => x.DropAsync(It.Is<DmsPath>(path => path.FullPath == "folder/file.txt"), 0))
            .Returns(ValueTask.CompletedTask);

        // When
        DmsProcessingResponse response = await dmsProcessingService.ProcessAsync(request);

        // Then
        response.StatusCode.Should().Be(204);
        dmsInstanceServiceMock.Verify(
            x => x.DropAsync(It.Is<DmsPath>(path => path.FullPath == "folder/file.txt"), 0),
            Times.Once
        );
        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }
}







