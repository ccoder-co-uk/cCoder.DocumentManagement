using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FolderRoleEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseFolderRoleAddEventAsync()
    {
        // Given
        FolderRole entity = CreateRandomFolderRole();
        folderRoleEventServiceMock
            .Setup(x => x.RaiseFolderRoleAddEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseFolderRoleAddEventAsync(entity);

        // Then
        folderRoleEventServiceMock.Verify(x => x.RaiseFolderRoleAddEventAsync(entity), Times.Once);
        folderRoleEventServiceMock.VerifyNoOtherCalls();
    }

}








