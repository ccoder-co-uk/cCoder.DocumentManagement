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
    public async Task ShouldDeleteRootFoldersThroughFolderOrchestrationWhenDeleteAsync()
    {
        // Given
        Guid rootFolderId = Guid.NewGuid();
        Guid childFolderId = Guid.NewGuid();

        Folder[] folders =
        [
            new Folder { Id = rootFolderId, AppId = 5, ParentId = null, Name = "Root", Path = "root" },
            new Folder { Id = childFolderId, AppId = 5, ParentId = rootFolderId, Name = "Child", Path = "root/child" },
            new Folder { Id = Guid.NewGuid(), AppId = 8, ParentId = null, Name = "Other", Path = "other" }
        ];

        folderOrchestrationServiceMock.Setup(x => x.GetAll(true)).Returns(folders.AsQueryable());
        folderOrchestrationServiceMock.Setup(x => x.DeleteAsync(rootFolderId)).Returns(ValueTask.CompletedTask);

        // When
        await service.DeleteAsync(5);

        // Then
        folderOrchestrationServiceMock.Verify(x => x.GetAll(true), Times.Once);
        folderOrchestrationServiceMock.Verify(x => x.DeleteAsync(rootFolderId), Times.Once);
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
            .Setup(x => x.AddOrUpdate(It.Is<IEnumerable<Folder>>(folders => folders.All(folder => folder.AppId == 7))))
            .Returns(ValueTask.FromResult<IEnumerable<cCoder.DocumentManagement.Models.Result<Folder>>>([]));

        // When
        await service.AddAsync(app);

        // Then
        folderOrchestrationServiceMock.VerifyAll();
    }
}
