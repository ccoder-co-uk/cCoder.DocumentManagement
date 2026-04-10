using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Orchestrations;

public partial class FileOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseUpdateEventAsyncWhenUpdateAsync()
    {
        // Given
        cCoder.Data.Models.DMS.File entity =
            Builder<cCoder.Data.Models.DMS.File>.CreateNew().Build();
        fileProcessingServiceMock.Setup(x => x.UpdateAsync(entity)).ReturnsAsync(entity);

        fileEventProcessingServiceMock
            .Setup(x => x.RaiseFileUpdateEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        cCoder.Data.Models.DMS.File result = await orchestrationService.UpdateAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        fileProcessingServiceMock.Verify(x => x.UpdateAsync(entity), Times.Once);
        fileEventProcessingServiceMock.Verify(x => x.RaiseFileUpdateEventAsync(entity), Times.Once);
    }

}







