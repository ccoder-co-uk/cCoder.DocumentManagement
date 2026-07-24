// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Orchestrations;

public partial class FileOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        cCoder.Data.Models.DMS.File entity =
            Builder<cCoder.Data.Models.DMS.File>.CreateNew()
            .Build();

        fileProcessingServiceMock.Setup(expression: x => x.AddFileAsync(newFile: entity))
            .ReturnsAsync(value: entity);

        fileEventProcessingServiceMock
            .Setup(expression: x => x.RaiseFileAddEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        cCoder.Data.Models.DMS.File result = await orchestrationService.AddFileAsync(newFile: entity);

        // Then
        result.Should()
            .BeSameAs(expected: entity);

        fileProcessingServiceMock.Verify(expression: x => x.AddFileAsync(newFile: entity), times: Times.Once);
        fileEventProcessingServiceMock.Verify(expression: x => x.RaiseFileAddEventAsync(entity: entity), times: Times.Once);
    }

}