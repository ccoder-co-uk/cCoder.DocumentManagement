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
    public async Task ShouldGetThenDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        // Given
        Guid id = Guid.NewGuid();
        FileContent entity = CreateRandomFileContent();
        fileContentProcessingServiceMock.Setup(x => x.Get(id)).Returns(entity);
        fileContentProcessingServiceMock.Setup(x => x.DeleteAsync(id)).Returns(ValueTask.CompletedTask);

        fileContentEventProcessingServiceMock
            .Setup(x => x.RaiseFileContentDeleteEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(id);

        // Then
        fileContentProcessingServiceMock.Verify(x => x.Get(id), Times.Once);
        fileContentProcessingServiceMock.Verify(x => x.DeleteAsync(id), Times.Once);
        fileContentEventProcessingServiceMock.Verify(x => x.RaiseFileContentDeleteEventAsync(entity), Times.Once);
    }

}








