using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FileProcessingServiceTests
{
    [Fact]
    public async Task ShouldDeleteEachFileWhenDeleteAllAsync()
    {
        // Given
        cCoder.Data.Models.DMS.File file = CreateRandomFile();
        fileServiceMock.Setup(x => x.DeleteAsync(file.Id)).Returns(ValueTask.CompletedTask);

        // When
        await fileProcessingService.DeleteAllAsync(new[] { file });

        // Then
        fileServiceMock.Verify(x => x.DeleteAsync(file.Id), Times.Once);
        fileServiceMock.VerifyNoOtherCalls();
    }

}







