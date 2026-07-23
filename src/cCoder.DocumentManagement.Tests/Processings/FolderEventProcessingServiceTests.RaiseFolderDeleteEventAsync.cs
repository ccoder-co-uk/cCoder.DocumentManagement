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

public partial class FolderEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseFolderDeleteEventAsync()
    {
        // Given
        Folder entity = CreateRandomFolder();
        folderEventServiceMock
            .Setup(expression: x => x.RaiseFolderDeleteEventAsync(entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseFolderDeleteEventAsync(entity: entity);

        // Then
        folderEventServiceMock.Verify(expression: x => x.RaiseFolderDeleteEventAsync(entity), times: Times.Once);
        folderEventServiceMock.VerifyNoOtherCalls();
    }

}