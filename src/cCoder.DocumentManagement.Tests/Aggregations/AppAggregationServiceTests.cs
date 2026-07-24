// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.DocumentManagement.Services.Aggregations;
using cCoder.DocumentManagement.Services.Orchestrations;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.DMS.Aggregations;

public partial class AppAggregationServiceTests
{
    private readonly Mock<IFolderOrchestrationService> folderOrchestrationServiceMock;
    private readonly AppAggregationService service;

    public AppAggregationServiceTests()
    {
        folderOrchestrationServiceMock = new Mock<IFolderOrchestrationService>(behavior: MockBehavior.Strict);
        service = new AppAggregationService(folderOrchestrationService: folderOrchestrationServiceMock.Object);
    }

    [Fact]
    public async Task ShouldDeleteAppFoldersThroughFolderOrchestrationWhenDeleteAsync()
    {
        // Given
        folderOrchestrationServiceMock
            .Setup(expression: x => x.DeleteAllByAppIdAsync(appId: 5))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.DeleteAsync(appId: 5);

        // Then
        folderOrchestrationServiceMock.Verify(expression: x => x.DeleteAllByAppIdAsync(appId: 5), times: Times.Once);
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
            .Setup(expression: x => x.AddOrUpdateForAppFolderAsync(items: It.Is<IEnumerable<Folder>>(match: folders => folders.All(predicate: folder => folder.AppId == 7))))
            .Returns(value: ValueTask.FromResult<IEnumerable<cCoder.DocumentManagement.Models.Result<Folder>>>(result: []));

        // When
        await service.AddAppAsync(newApp: app);

        // Then
        folderOrchestrationServiceMock.VerifyAll();
    }
}
