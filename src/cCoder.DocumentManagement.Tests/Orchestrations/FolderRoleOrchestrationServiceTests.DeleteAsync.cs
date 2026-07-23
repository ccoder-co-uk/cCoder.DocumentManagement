// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Orchestrations;

public partial class FolderRoleOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        // Given
        FolderRole folderRole = CreateRandomFolderRole();

        folderRoleProcessingServiceMock.Setup(expression: x => x.DeleteAsync(entity: folderRole))
            .Returns(value: ValueTask.CompletedTask);

        folderRoleEventProcessingServiceMock
            .Setup(expression: x => x.RaiseFolderRoleDeleteEventAsync(entity: folderRole))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(entity: folderRole);

        // Then
        folderRoleProcessingServiceMock.Verify(expression: x => x.DeleteAsync(entity: folderRole), times: Times.Once);
        folderRoleEventProcessingServiceMock.Verify(expression: x => x.RaiseFolderRoleDeleteEventAsync(entity: folderRole), times: Times.Once);
    }

}