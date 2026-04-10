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
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        Folder entity = CreateRandomFolder();
        folderProcessingServiceMock.Setup(x => x.AddAsync(entity)).ReturnsAsync(entity);

        folderEventProcessingServiceMock
            .Setup(x => x.RaiseFolderAddEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        Folder result = await orchestrationService.AddAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        folderProcessingServiceMock.Verify(x => x.AddAsync(entity), Times.Once);
        folderEventProcessingServiceMock.Verify(x => x.RaiseFolderAddEventAsync(entity), Times.Once);
    }

}








