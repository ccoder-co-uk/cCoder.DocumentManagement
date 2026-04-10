using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Orchestrations;

public partial class FolderRoleOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        FolderRole entity = CreateRandomFolderRole();
        folderRoleProcessingServiceMock.Setup(x => x.AddAsync(entity)).ReturnsAsync(entity);

        folderRoleEventProcessingServiceMock
            .Setup(x => x.RaiseFolderRoleAddEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        FolderRole result = await orchestrationService.AddAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        folderRoleProcessingServiceMock.Verify(x => x.AddAsync(entity), Times.Once);
        folderRoleEventProcessingServiceMock.Verify(x => x.RaiseFolderRoleAddEventAsync(entity), Times.Once);
    }

}








