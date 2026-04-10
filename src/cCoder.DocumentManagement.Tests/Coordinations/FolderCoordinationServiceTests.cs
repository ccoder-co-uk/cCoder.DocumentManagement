using cCoder.DocumentManagement.Services.Coordinations;
using cCoder.DocumentManagement.Services.Orchestrations;
using cCoder.Data.Models.DMS;
using Moq;
using Xunit;
using File = cCoder.Data.Models.DMS.File;

namespace cCoder.Core.Services.Tests.DMS.Coordinations;

public class FolderCoordinationServiceTests
{
    private readonly Mock<IFolderOrchestrationService> folderOrchestrationServiceMock;
    private readonly Mock<IFileOrchestrationService> fileOrchestrationServiceMock;
    private readonly FolderCoordinationService coordinationService;

    public FolderCoordinationServiceTests()
    {
        folderOrchestrationServiceMock = new Mock<IFolderOrchestrationService>(MockBehavior.Strict);
        fileOrchestrationServiceMock = new Mock<IFileOrchestrationService>(MockBehavior.Strict);
        coordinationService = new FolderCoordinationService(
            folderOrchestrationServiceMock.Object,
            fileOrchestrationServiceMock.Object);
    }

    [Fact]
    public async Task ShouldDeleteDirectChildFilesThenChildFoldersWhenDeleteFolderAsync()
    {
        // Given
        Guid folderId = Guid.NewGuid();
        Guid childFolderId = Guid.NewGuid();
        Guid childFileId = Guid.NewGuid();
        Folder folder = new()
        {
            Id = folderId,
            AppId = 1,
            Name = "root",
            Path = "root"
        };

        fileOrchestrationServiceMock
            .Setup(service => service.GetAll(true))
            .Returns(new[]
            {
                new File
                {
                    Id = childFileId,
                    FolderId = folderId,
                    Name = "child.txt",
                    Path = "root/child.txt"
                }
            }.AsQueryable());

        folderOrchestrationServiceMock
            .Setup(service => service.GetAll(true))
            .Returns(new[]
            {
                folder,
                new Folder
                {
                    Id = childFolderId,
                    AppId = 1,
                    ParentId = folderId,
                    Name = "child",
                    Path = "root/child"
                }
            }.AsQueryable());

        fileOrchestrationServiceMock
            .Setup(service => service.DeleteAsync(childFileId))
            .Returns(ValueTask.CompletedTask);

        folderOrchestrationServiceMock
            .Setup(service => service.DeleteAsync(childFolderId))
            .Returns(ValueTask.CompletedTask);

        // When
        await coordinationService.DeleteFolderAsync(folder);

        // Then
        fileOrchestrationServiceMock.Verify(service => service.GetAll(true), Times.Once);
        folderOrchestrationServiceMock.Verify(service => service.GetAll(true), Times.Once);
        fileOrchestrationServiceMock.Verify(service => service.DeleteAsync(childFileId), Times.Once);
        folderOrchestrationServiceMock.Verify(service => service.DeleteAsync(childFolderId), Times.Once);
    }

    [Fact]
    public async Task ShouldDoNothingWhenDeleteFolderAsyncWithNullFolder()
    {
        // When
        await coordinationService.DeleteFolderAsync(null);

        // Then
        fileOrchestrationServiceMock.VerifyNoOtherCalls();
        folderOrchestrationServiceMock.VerifyNoOtherCalls();
    }
}
