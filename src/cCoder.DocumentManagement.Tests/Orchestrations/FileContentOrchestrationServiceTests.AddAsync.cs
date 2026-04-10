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
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        FileContent entity = CreateRandomFileContent();
        fileContentProcessingServiceMock.Setup(x => x.AddAsync(entity)).ReturnsAsync(entity);

        fileContentEventProcessingServiceMock
            .Setup(x => x.RaiseFileContentAddEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        FileContent result = await orchestrationService.AddAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        fileContentProcessingServiceMock.Verify(x => x.AddAsync(entity), Times.Once);
        fileContentEventProcessingServiceMock.Verify(x => x.RaiseFileContentAddEventAsync(entity), Times.Once);
    }

}








