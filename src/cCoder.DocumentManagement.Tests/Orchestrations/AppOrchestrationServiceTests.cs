using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.DocumentManagement.Services.Orchestrations;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.DMS.Orchestrations;

public class AppOrchestrationServiceTests
{
    private readonly Mock<IFolderOrchestrationService> folderOrchestrationServiceMock;
    private readonly AppOrchestrationService service;

    public AppOrchestrationServiceTests()
    {
        folderOrchestrationServiceMock = new Mock<IFolderOrchestrationService>(MockBehavior.Strict);
        service = new AppOrchestrationService(folderOrchestrationServiceMock.Object);
    }

    [Fact]
    public async Task ShouldDeleteAppFoldersThroughFolderOrchestrationWhenDeleteAsync()
    {
        // Given
        folderOrchestrationServiceMock
            .Setup(x => x.DeleteAllByAppIdAsync(5))
            .Returns(ValueTask.CompletedTask);

        // When
        await service.DeleteAsync(5);

        // Then
        folderOrchestrationServiceMock.Verify(x => x.DeleteAllByAppIdAsync(5), Times.Once);
        folderOrchestrationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldStampFolderAppIdsWhenAddAsync()
    {
        // Given
        App app = new()
        {
            Id = 7,
            Folders =
            [
                new Folder { Id = Guid.NewGuid(), Name = "Root", Path = "root" },
                new Folder { Id = Guid.NewGuid(), Name = "Child", Path = "root/child" }
            ]
        };

        folderOrchestrationServiceMock
            .Setup(x => x.AddOrUpdateForAppAsync(It.Is<IEnumerable<Folder>>(folders => folders.All(folder => folder.AppId == 7))))
            .Returns(ValueTask.FromResult<IEnumerable<cCoder.DocumentManagement.Models.Result<Folder>>>([]));

        // When
        await service.AddAsync(app);

        // Then
        folderOrchestrationServiceMock.VerifyAll();
    }
}
