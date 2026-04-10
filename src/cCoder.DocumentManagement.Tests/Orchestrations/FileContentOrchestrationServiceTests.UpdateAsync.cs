using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Orchestrations;

public partial class FileContentOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseUpdateEventAsyncWhenUpdateAsync()
    {
        // Given
        FileContent entity = CreateRandomFileContent();
        fileContentProcessingServiceMock.Setup(x => x.UpdateAsync(entity)).ReturnsAsync(entity);

        fileContentEventProcessingServiceMock
            .Setup(x => x.RaiseFileContentUpdateEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        FileContent result = await orchestrationService.UpdateAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        fileContentProcessingServiceMock.Verify(x => x.UpdateAsync(entity), Times.Once);
        fileContentEventProcessingServiceMock.Verify(x => x.RaiseFileContentUpdateEventAsync(entity), Times.Once);
    }

}








