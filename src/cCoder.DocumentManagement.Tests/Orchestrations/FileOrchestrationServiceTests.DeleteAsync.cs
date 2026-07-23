// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FizzWare.NBuilder;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Orchestrations;

public partial class FileOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldGetThenDeleteThenRaiseDeleteEventAsyncWhenDeleteAsync()
    {
        // Given
        Guid id = Guid.NewGuid();

        cCoder.Data.Models.DMS.File entity =
            Builder<cCoder.Data.Models.DMS.File>.CreateNew()
            .Build();

        entity.Id = id;

        fileProcessingServiceMock.Setup(expression: x => x.GetAll(ignoreFilters: true))
            .Returns(value: new[] { entity }.AsQueryable());

        fileProcessingServiceMock.Setup(expression: x => x.DeleteAsync(id: id))
            .Returns(value: ValueTask.CompletedTask);

        fileEventProcessingServiceMock
            .Setup(expression: x => x.RaiseFileDeleteEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAsync(id: id);

        // Then
        fileProcessingServiceMock.Verify(expression: x => x.GetAll(ignoreFilters: true), times: Times.Once);
        fileProcessingServiceMock.Verify(expression: x => x.DeleteAsync(id: id), times: Times.Once);
        fileEventProcessingServiceMock.Verify(expression: x => x.RaiseFileDeleteEventAsync(entity: entity), times: Times.Once);
    }

}