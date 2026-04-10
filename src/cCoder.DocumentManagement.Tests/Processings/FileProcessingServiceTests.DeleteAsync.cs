using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FileProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenDeleteAsync()
    {
        // Given
        Guid id = Guid.NewGuid();
        fileServiceMock.Setup(x => x.DeleteAsync(id)).Returns(ValueTask.CompletedTask);

        // When
        await fileProcessingService.DeleteAsync(id);

        // Then
        fileServiceMock.Verify(x => x.DeleteAsync(id), Times.Once);
        fileServiceMock.VerifyNoOtherCalls();
    }

}




