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

public partial class FileContentOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseUpdateEventAsyncWhenUpdateAsync()
    {
        // Given
        FileContent entity = CreateRandomFileContent();

        fileContentProcessingServiceMock.Setup(expression: x => x.UpdateFileContentAsync(entity: entity))
            .ReturnsAsync(value: entity);

        fileContentEventProcessingServiceMock
            .Setup(expression: x => x.RaiseFileContentUpdateEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        FileContent result = await orchestrationService.UpdateFileContentAsync(entity: entity);

        // Then
        result.Should()
            .BeSameAs(expected: entity);

        fileContentProcessingServiceMock.Verify(expression: x => x.UpdateFileContentAsync(entity: entity), times: Times.Once);
        fileContentEventProcessingServiceMock.Verify(expression: x => x.RaiseFileContentUpdateEventAsync(entity: entity), times: Times.Once);
    }

}