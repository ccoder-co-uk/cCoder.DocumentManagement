using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Orchestrations;

public partial class FolderOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseUpdateEventAsyncWhenUpdateAsync()
    {
        // Given
        Folder entity = CreateRandomFolder();
        folderProcessingServiceMock.Setup(x => x.UpdateAsync(entity)).ReturnsAsync(entity);

        folderEventProcessingServiceMock
            .Setup(x => x.RaiseFolderUpdateEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        Folder result = await orchestrationService.UpdateAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        folderProcessingServiceMock.Verify(x => x.UpdateAsync(entity), Times.Once);
        folderEventProcessingServiceMock.Verify(x => x.RaiseFolderUpdateEventAsync(entity), Times.Once);
    }

}








