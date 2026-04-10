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
    public async Task ShouldPassThroughCallWhenRaiseFolderRoleDeleteEventAsync()
    {
        // Given
        FolderRole entity = CreateRandomFolderRole();
        folderRoleEventServiceMock
            .Setup(x => x.RaiseFolderRoleDeleteEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseFolderRoleDeleteEventAsync(entity);

        // Then
        folderRoleEventServiceMock.Verify(x => x.RaiseFolderRoleDeleteEventAsync(entity), Times.Once);
        folderRoleEventServiceMock.VerifyNoOtherCalls();
    }

}








