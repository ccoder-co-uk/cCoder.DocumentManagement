// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FolderRoleEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseFolderRoleDeleteEventAsync()
    {
        // Given
        FolderRole entity = CreateRandomFolderRole();

        folderRoleEventServiceMock
            .Setup(expression: x => x.RaiseFolderRoleDeleteEventAsync(folderRole: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseFolderRoleDeleteEventAsync(folderRole: entity);

        // Then
        folderRoleEventServiceMock.Verify(expression: x => x.RaiseFolderRoleDeleteEventAsync(folderRole: entity), times: Times.Once);
        folderRoleEventServiceMock.VerifyNoOtherCalls();
    }

}