using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Orchestrations;

public partial class FileContentOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDelegateToProcessingServiceWhenDeleteAllAsync()
    {
        // Given
        FileContent[] entities = [CreateRandomFileContent()];
        fileContentProcessingServiceMock.Setup(x => x.DeleteAllAsync(entities)).Returns(ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAllAsync(entities);

        // Then
        fileContentProcessingServiceMock.Verify(x => x.DeleteAllAsync(entities), Times.Once);
        fileContentProcessingServiceMock.VerifyNoOtherCalls();
        fileContentEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}








