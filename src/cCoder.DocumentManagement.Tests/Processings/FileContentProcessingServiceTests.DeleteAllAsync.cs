using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FileContentProcessingServiceTests
{
    [Fact]
    public async Task ShouldDeleteEachItemWhenDeleteAllAsync()
    {
        // Given
        FileContent fileContent = CreateRandomFileContent();
        fileContentServiceMock
            .Setup(x => x.DeleteAsync(fileContent.Id))
            .Returns(ValueTask.CompletedTask);

        // When
        await fileContentProcessingService.DeleteAllAsync(new[] { fileContent });

        // Then
        fileContentServiceMock.Verify(x => x.DeleteAsync(fileContent.Id), Times.Once);
        fileContentServiceMock.VerifyNoOtherCalls();
    }

}








