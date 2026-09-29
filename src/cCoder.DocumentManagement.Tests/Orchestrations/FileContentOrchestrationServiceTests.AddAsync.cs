// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.DMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Orchestrations;

public partial class FileContentOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        FileContent entity = CreateRandomFileContent();

        fileContentProcessingServiceMock.Setup(expression: x => x.AddFileContentAsync(newFileContent: entity))
            .ReturnsAsync(value: entity);

        fileContentEventProcessingServiceMock
            .Setup(expression: x => x.RaiseFileContentAddEventAsync(fileContent: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        FileContent result = await orchestrationService.AddFileContentAsync(newFileContent: entity);

        // Then
        result.Should()
            .BeSameAs(expected: entity);

        fileContentProcessingServiceMock.Verify(expression: x => x.AddFileContentAsync(newFileContent: entity), times: Times.Once);
        fileContentEventProcessingServiceMock.Verify(expression: x => x.RaiseFileContentAddEventAsync(fileContent: entity), times: Times.Once);
    }

}