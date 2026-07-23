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

public partial class FileContentOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldGetThenDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        // Given
        Guid id = Guid.NewGuid();
        FileContent entity = CreateRandomFileContent();

        fileContentProcessingServiceMock.Setup(expression: x => x.Get(id: id))
            .Returns(value: entity);

        fileContentProcessingServiceMock.Setup(expression: x => x.DeleteAsync(id: id))
            .Returns(value: ValueTask.CompletedTask);

        fileContentEventProcessingServiceMock
            .Setup(expression: x => x.RaiseFileContentDeleteEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(id: id);

        // Then
        fileContentProcessingServiceMock.Verify(expression: x => x.Get(id: id), times: Times.Once);
        fileContentProcessingServiceMock.Verify(expression: x => x.DeleteAsync(id: id), times: Times.Once);
        fileContentEventProcessingServiceMock.Verify(expression: x => x.RaiseFileContentDeleteEventAsync(entity: entity), times: Times.Once);
    }

}