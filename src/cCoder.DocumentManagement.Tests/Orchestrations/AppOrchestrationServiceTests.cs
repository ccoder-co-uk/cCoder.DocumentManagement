using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.DocumentManagement.Services.Orchestrations;
using cCoder.DocumentManagement.Services.Processings;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.DMS.Orchestrations;

public class AppOrchestrationServiceTests
{
    private readonly Mock<IFolderProcessingService> folderProcessingServiceMock;
    private readonly AppOrchestrationService service;

    public AppOrchestrationServiceTests()
    {
        folderProcessingServiceMock = new Mock<IFolderProcessingService>(MockBehavior.Strict);
        service = new AppOrchestrationService(folderProcessingServiceMock.Object);
    }

    [Fact]
    public async Task ShouldDeleteAppFoldersThroughFolderProcessingWhenDeleteAsync()
    {
        // Given
        folderProcessingServiceMock
            .Setup(x => x.DeleteByAppIdAsync(5))
            .Returns(ValueTask.CompletedTask);

        // When
        await service.DeleteAsync(5);

        // Then
        folderProcessingServiceMock.Verify(x => x.DeleteByAppIdAsync(5), Times.Once);
        folderProcessingServiceMock.VerifyNoOtherCalls();
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

        folderProcessingServiceMock
            .Setup(x => x.AddOrUpdateForAppAsync(It.Is<IEnumerable<Folder>>(folders => folders.All(folder => folder.AppId == 7))))
            .Returns(ValueTask.FromResult<IEnumerable<cCoder.DocumentManagement.Models.Result<Folder>>>([]));

        // When
        await service.AddAsync(app);

        // Then
        folderProcessingServiceMock.VerifyAll();
    }
}
