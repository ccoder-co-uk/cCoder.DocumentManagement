// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        folderOrchestrationServiceMock = new Mock<IFolderOrchestrationService>(behavior: MockBehavior.Strict);
        service = new AppOrchestrationService(folderOrchestrationService: folderOrchestrationServiceMock.Object);
    }

    [Fact]
    public async Task ShouldDeleteAppFoldersThroughFolderOrchestrationWhenDeleteAsync()
    {
        // Given
        folderOrchestrationServiceMock
            .Setup(expression: x => x.DeleteAllByAppIdAsync(5))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.DeleteAsync(appId: 5);

        // Then
        folderOrchestrationServiceMock.Verify(expression: x => x.DeleteAllByAppIdAsync(5), times: Times.Once);
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
            .Setup(expression: x => x.AddOrUpdateForAppAsync(It.Is<IEnumerable<Folder>>(folders => folders.All(folder => folder.AppId == 7))))
            .Returns(value: ValueTask.FromResult<IEnumerable<cCoder.DocumentManagement.Models.Result<Folder>>>([]));

        // When
        await service.AddAsync(app: app);

        // Then
        folderOrchestrationServiceMock.VerifyAll();
    }
}