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
    public async Task ShouldPassThroughCallWhenRaiseFolderUpdateEventAsync()
    {
        // Given
        Folder entity = CreateRandomFolder();
        folderEventServiceMock
            .Setup(x => x.RaiseFolderUpdateEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseFolderUpdateEventAsync(entity);

        // Then
        folderEventServiceMock.Verify(x => x.RaiseFolderUpdateEventAsync(entity), Times.Once);
        folderEventServiceMock.VerifyNoOtherCalls();
    }

}








