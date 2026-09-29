// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.Data.Models.DMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Orchestrations;

public partial class FileContentOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultsWhenGetAll()
    {
        // Given
        IQueryable<FileContent> entities = new[] { CreateRandomFileContent() }.AsQueryable();

        fileContentProcessingServiceMock.Setup(expression: x => x.GetAll(ignoreFilters: true))
            .Returns(value: entities);

        // When
        IQueryable<FileContent> result = orchestrationService.GetAll(ignoreFilters: true);

        // Then
        result.Should()
            .BeSameAs(expected: entities);

        fileContentProcessingServiceMock.Verify(expression: x => x.GetAll(ignoreFilters: true), times: Times.Once);
        fileContentProcessingServiceMock.VerifyNoOtherCalls();
        fileContentEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}