using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FileProcessingServiceTests
{
    [Fact]
    public async Task ShouldDeleteAllContentsWhenHandleFileDeleteEventAsync()
    {
        // Given
        cCoder.Data.Models.DMS.File file = CreateRandomFile();
        fileContentServiceMock
            .Setup(x => x.DeleteAllForFileAsync(file.Id))
            .Returns(ValueTask.CompletedTask);

        // When
        await fileProcessingService.HandleFileDeleteEventAsync(file);

        // Then
        fileContentServiceMock.Verify(x => x.DeleteAllForFileAsync(file.Id), Times.Once);
        fileContentServiceMock.VerifyNoOtherCalls();
        fileServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldDeleteEmptySetWhenFileHasNoContentsForHandleFileDeleteEventAsync()
    {
        // Given
        cCoder.Data.Models.DMS.File file = CreateRandomFile();
        fileContentServiceMock
            .Setup(x => x.DeleteAllForFileAsync(file.Id))
            .Returns(ValueTask.CompletedTask);

        // When
        await fileProcessingService.HandleFileDeleteEventAsync(file);

        // Then
        fileContentServiceMock.Verify(x => x.DeleteAllForFileAsync(file.Id), Times.Once);
        fileContentServiceMock.VerifyNoOtherCalls();
        fileServiceMock.VerifyNoOtherCalls();
    }

}








