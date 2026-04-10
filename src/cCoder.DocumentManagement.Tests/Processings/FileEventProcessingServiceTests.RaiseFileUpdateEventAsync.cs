using Moq;
using Xunit;
using FileEntity = cCoder.Data.Models.DMS.File;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FileEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseFileUpdateEventAsync()
    {
        // Given
        FileEntity entity = CreateRandomFileEntity();
        fileEventServiceMock
            .Setup(x => x.RaiseFileUpdateEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseFileUpdateEventAsync(entity);

        // Then
        fileEventServiceMock.Verify(x => x.RaiseFileUpdateEventAsync(entity), Times.Once);
        fileEventServiceMock.VerifyNoOtherCalls();
    }

}








