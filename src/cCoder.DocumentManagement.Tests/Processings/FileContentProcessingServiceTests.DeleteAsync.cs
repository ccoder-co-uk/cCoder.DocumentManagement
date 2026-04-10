using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FileContentProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenDeleteAsync()
    {
        // Given
        Guid id = Guid.NewGuid();
        fileContentServiceMock.Setup(x => x.DeleteAsync(id)).Returns(ValueTask.CompletedTask);

        // When
        await fileContentProcessingService.DeleteAsync(id);

        // Then
        fileContentServiceMock.Verify(x => x.DeleteAsync(id), Times.Once);
        fileContentServiceMock.VerifyNoOtherCalls();
    }

}





