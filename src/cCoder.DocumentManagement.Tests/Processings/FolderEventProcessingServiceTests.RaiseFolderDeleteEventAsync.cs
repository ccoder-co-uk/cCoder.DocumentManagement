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
    public async Task ShouldPassThroughCallWhenRaiseFolderDeleteEventAsync()
    {
        // Given
        Folder entity = CreateRandomFolder();
        folderEventServiceMock
            .Setup(x => x.RaiseFolderDeleteEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseFolderDeleteEventAsync(entity);

        // Then
        folderEventServiceMock.Verify(x => x.RaiseFolderDeleteEventAsync(entity), Times.Once);
        folderEventServiceMock.VerifyNoOtherCalls();
    }

}








