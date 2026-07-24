// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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

        folderRoleProcessingServiceMock.Setup(expression: x => x.AddFolderRoleAsync(newFolderRole: entity))
            .ReturnsAsync(value: entity);

        folderRoleEventProcessingServiceMock
            .Setup(expression: x => x.RaiseFolderRoleAddEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        FolderRole result = await orchestrationService.AddFolderRoleAsync(newFolderRole: entity);

        // Then
        result.Should()
            .BeSameAs(expected: entity);

        folderRoleProcessingServiceMock.Verify(expression: x => x.AddFolderRoleAsync(newFolderRole: entity), times: Times.Once);
        folderRoleEventProcessingServiceMock.Verify(expression: x => x.RaiseFolderRoleAddEventAsync(entity: entity), times: Times.Once);
    }

}