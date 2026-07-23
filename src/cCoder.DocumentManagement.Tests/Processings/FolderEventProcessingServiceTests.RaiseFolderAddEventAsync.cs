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
    public async Task ShouldPassThroughCallWhenRaiseFolderAddEventAsync()
    {
        // Given
        Folder entity = CreateRandomFolder();
        folderEventServiceMock
            .Setup(expression: x => x.RaiseFolderAddEventAsync(entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseFolderAddEventAsync(entity: entity);

        // Then
        folderEventServiceMock.Verify(expression: x => x.RaiseFolderAddEventAsync(entity), times: Times.Once);
        folderEventServiceMock.VerifyNoOtherCalls();
    }

}