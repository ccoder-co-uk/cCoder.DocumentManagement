using FizzWare.NBuilder;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Orchestrations;

public partial class FileOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldGetThenDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        // Given
        Guid id = Guid.NewGuid();
        cCoder.Data.Models.DMS.File entity =
            Builder<cCoder.Data.Models.DMS.File>.CreateNew().Build();
        entity.Id = id;
        fileProcessingServiceMock.Setup(x => x.GetAll(true)).Returns(new[] { entity }.AsQueryable());
        fileProcessingServiceMock.Setup(x => x.DeleteAsync(id)).Returns(ValueTask.CompletedTask);

        fileEventProcessingServiceMock
            .Setup(x => x.RaiseFileDeleteEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(id);

        // Then
        fileProcessingServiceMock.Verify(x => x.GetAll(true), Times.Once);
        fileProcessingServiceMock.Verify(x => x.DeleteAsync(id), Times.Once);
        fileEventProcessingServiceMock.Verify(x => x.RaiseFileDeleteEventAsync(entity), Times.Once);
    }

}







