// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Services.Coordinations;
using cCoder.DocumentManagement.Services.Orchestrations;
using cCoder.Data.Models.DMS;
using Moq;
using Xunit;
using File = cCoder.Data.Models.DMS.File;

namespace cCoder.Core.Services.Tests.DMS.Coordinations;

public partial class FolderCoordinationServiceTests
{
    private readonly Mock<IFolderOrchestrationService> folderOrchestrationServiceMock;
    private readonly Mock<IFileOrchestrationService> fileOrchestrationServiceMock;
    private readonly FolderCoordinationService coordinationService;

    public FolderCoordinationServiceTests()
    {
        folderOrchestrationServiceMock = new Mock<IFolderOrchestrationService>(behavior: MockBehavior.Strict);
        fileOrchestrationServiceMock = new Mock<IFileOrchestrationService>(behavior: MockBehavior.Strict);
        coordinationService = new FolderCoordinationService(
            folderOrchestrationService: folderOrchestrationServiceMock.Object,
            fileOrchestrationService: fileOrchestrationServiceMock.Object);
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
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(value: new[]
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
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(value: new[]
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
            .Setup(expression: service => service.DeleteAsync(fileId: childFileId))
            .Returns(value: ValueTask.CompletedTask);

        folderOrchestrationServiceMock
            .Setup(expression: service => service.DeleteAsync(folderId: childFolderId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await coordinationService.DeleteFolderAsync(deletedFolder: folder);

        // Then
        fileOrchestrationServiceMock.Verify(expression: service => service.GetAll(ignoreFilters: true), times: Times.Once);
        folderOrchestrationServiceMock.Verify(expression: service => service.GetAll(ignoreFilters: true), times: Times.Once);
        fileOrchestrationServiceMock.Verify(expression: service => service.DeleteAsync(fileId: childFileId), times: Times.Once);
        folderOrchestrationServiceMock.Verify(expression: service => service.DeleteAsync(folderId: childFolderId), times: Times.Once);
    }

    [Fact]
    public async Task ShouldDoNothingWhenDeleteFolderAsyncWithNullFolder()
    {
        // When
        // Given
        await coordinationService.DeleteFolderAsync(deletedFolder: null);

        // Then
        fileOrchestrationServiceMock.VerifyNoOtherCalls();
        folderOrchestrationServiceMock.VerifyNoOtherCalls();
    }
}