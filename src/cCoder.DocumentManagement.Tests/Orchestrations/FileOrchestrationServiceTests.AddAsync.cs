using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Orchestrations;

public partial class FileOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        cCoder.Data.Models.DMS.File entity =
            Builder<cCoder.Data.Models.DMS.File>.CreateNew().Build();
        fileProcessingServiceMock.Setup(x => x.AddAsync(entity)).ReturnsAsync(entity);

        fileEventProcessingServiceMock
            .Setup(x => x.RaiseFileAddEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        cCoder.Data.Models.DMS.File result = await orchestrationService.AddAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        fileProcessingServiceMock.Verify(x => x.AddAsync(entity), Times.Once);
        fileEventProcessingServiceMock.Verify(x => x.RaiseFileAddEventAsync(entity), Times.Once);
    }

}







