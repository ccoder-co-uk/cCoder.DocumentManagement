using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FileContentEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseFileContentUpdateEventAsync()
    {
        // Given
        FileContent entity = CreateRandomFileContent();
        fileContentEventServiceMock
            .Setup(x => x.RaiseFileContentUpdateEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseFileContentUpdateEventAsync(entity);

        // Then
        fileContentEventServiceMock.Verify(x => x.RaiseFileContentUpdateEventAsync(entity), Times.Once);
        fileContentEventServiceMock.VerifyNoOtherCalls();
    }

}








