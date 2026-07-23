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

public partial class FolderOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldGetThenDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        // Given
        Guid id = Guid.NewGuid();
        Folder entity = CreateRandomFolder();
        entity.Id = id;
        folderProcessingServiceMock.Setup(expression: x => x.GetAll(true)).Returns(value: new[] { entity }.AsQueryable());
        folderProcessingServiceMock.Setup(expression: x => x.DeleteAsync(id)).Returns(value: ValueTask.CompletedTask);

        folderEventProcessingServiceMock
            .Setup(expression: x => x.RaiseFolderDeleteEventAsync(entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(id: id);

        // Then
        folderProcessingServiceMock.Verify(expression: x => x.GetAll(true), times: Times.Once);
        folderProcessingServiceMock.Verify(expression: x => x.DeleteAsync(id), times: Times.Once);
        folderEventProcessingServiceMock.Verify(expression: x => x.RaiseFolderDeleteEventAsync(entity), times: Times.Once);
    }

}