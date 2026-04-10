using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Orchestrations;

public partial class FolderOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldGetThenDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        // Given
        Guid id = Guid.NewGuid();
        Folder entity = CreateRandomFolder();
        entity.Id = id;
        folderProcessingServiceMock.Setup(x => x.GetAll(true)).Returns(new[] { entity }.AsQueryable());
        folderProcessingServiceMock.Setup(x => x.DeleteAsync(id)).Returns(ValueTask.CompletedTask);

        folderEventProcessingServiceMock
            .Setup(x => x.RaiseFolderDeleteEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(id);

        // Then
        folderProcessingServiceMock.Verify(x => x.GetAll(true), Times.Once);
        folderProcessingServiceMock.Verify(x => x.DeleteAsync(id), Times.Once);
        folderEventProcessingServiceMock.Verify(x => x.RaiseFolderDeleteEventAsync(entity), Times.Once);
    }

}








