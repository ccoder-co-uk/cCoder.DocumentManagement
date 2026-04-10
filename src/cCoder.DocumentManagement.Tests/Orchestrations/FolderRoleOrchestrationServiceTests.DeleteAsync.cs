using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Orchestrations;

public partial class FolderRoleOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        // Given
        FolderRole folderRole = CreateRandomFolderRole();
        folderRoleProcessingServiceMock.Setup(x => x.DeleteAsync(folderRole)).Returns(ValueTask.CompletedTask);

        folderRoleEventProcessingServiceMock
            .Setup(x => x.RaiseFolderRoleDeleteEventAsync(folderRole))
            .Returns(ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(folderRole);

        // Then
        folderRoleProcessingServiceMock.Verify(x => x.DeleteAsync(folderRole), Times.Once);
        folderRoleEventProcessingServiceMock.Verify(x => x.RaiseFolderRoleDeleteEventAsync(folderRole), Times.Once);
    }

}








