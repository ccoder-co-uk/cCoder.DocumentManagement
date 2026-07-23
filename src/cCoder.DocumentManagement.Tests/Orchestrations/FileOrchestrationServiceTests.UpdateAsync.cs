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
    public async Task ShouldCallProcessingThenRaiseUpdateEventAsyncWhenUpdateAsync()
    {
        // Given
        cCoder.Data.Models.DMS.File entity =
            Builder<cCoder.Data.Models.DMS.File>.CreateNew().Build();
        fileProcessingServiceMock.Setup(expression: x => x.UpdateAsync(entity)).ReturnsAsync(value: entity);

        fileEventProcessingServiceMock
            .Setup(expression: x => x.RaiseFileUpdateEventAsync(entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        cCoder.Data.Models.DMS.File result = await orchestrationService.UpdateAsync(entity: entity);

        // Then
        result.Should().BeSameAs(expected: entity);
        fileProcessingServiceMock.Verify(expression: x => x.UpdateAsync(entity), times: Times.Once);
        fileEventProcessingServiceMock.Verify(expression: x => x.RaiseFileUpdateEventAsync(entity), times: Times.Once);
    }

}