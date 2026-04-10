using Moq;
using Xunit;
using FileEntity = cCoder.Data.Models.DMS.File;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FileEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseFileAddEventAsync()
    {
        // Given
        FileEntity entity = CreateRandomFileEntity();
        fileEventServiceMock
            .Setup(x => x.RaiseFileAddEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseFileAddEventAsync(entity);

        // Then
        fileEventServiceMock.Verify(x => x.RaiseFileAddEventAsync(entity), Times.Once);
        fileEventServiceMock.VerifyNoOtherCalls();
    }

}








