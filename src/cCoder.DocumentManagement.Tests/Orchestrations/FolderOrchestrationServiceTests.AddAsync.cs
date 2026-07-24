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

public partial class FolderOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        Folder entity = CreateRandomFolder();

        folderProcessingServiceMock.Setup(expression: x => x.AddFolderAsync(newFolder: entity))
            .ReturnsAsync(value: entity);

        folderEventProcessingServiceMock
            .Setup(expression: x => x.RaiseFolderAddEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        Folder result = await orchestrationService.AddFolderAsync(newFolder: entity);

        // Then
        result.Should()
            .BeSameAs(expected: entity);

        folderProcessingServiceMock.Verify(expression: x => x.AddFolderAsync(newFolder: entity), times: Times.Once);
        folderEventProcessingServiceMock.Verify(expression: x => x.RaiseFolderAddEventAsync(entity: entity), times: Times.Once);
    }

}