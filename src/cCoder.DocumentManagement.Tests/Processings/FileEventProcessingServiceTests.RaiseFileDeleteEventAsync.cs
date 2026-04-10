using Moq;
using Xunit;
using FileEntity = cCoder.Data.Models.DMS.File;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FileEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseFileDeleteEventAsync()
    {
        // Given
        FileEntity entity = CreateRandomFileEntity();
        fileEventServiceMock
            .Setup(x => x.RaiseFileDeleteEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseFileDeleteEventAsync(entity);

        // Then
        fileEventServiceMock.Verify(x => x.RaiseFileDeleteEventAsync(entity), Times.Once);
        fileEventServiceMock.VerifyNoOtherCalls();
    }

}








