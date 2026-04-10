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
    public async Task ShouldPassThroughCallWhenRaiseFileContentDeleteEventAsync()
    {
        // Given
        FileContent entity = CreateRandomFileContent();
        fileContentEventServiceMock
            .Setup(x => x.RaiseFileContentDeleteEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseFileContentDeleteEventAsync(entity);

        // Then
        fileContentEventServiceMock.Verify(x => x.RaiseFileContentDeleteEventAsync(entity), Times.Once);
        fileContentEventServiceMock.VerifyNoOtherCalls();
    }

}








