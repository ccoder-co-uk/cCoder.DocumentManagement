using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FolderEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseFolderAddEventAsync()
    {
        // Given
        Folder entity = CreateRandomFolder();
        folderEventServiceMock
            .Setup(x => x.RaiseFolderAddEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseFolderAddEventAsync(entity);

        // Then
        folderEventServiceMock.Verify(x => x.RaiseFolderAddEventAsync(entity), Times.Once);
        folderEventServiceMock.VerifyNoOtherCalls();
    }

}








