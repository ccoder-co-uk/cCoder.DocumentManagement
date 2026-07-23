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
    public void ShouldReturnProcessingResultWhenGet()
    {
        // Given
        Guid id = Guid.NewGuid();
        FileContent entity = CreateRandomFileContent();

        fileContentProcessingServiceMock.Setup(expression: x => x.Get(id: id))
            .Returns(value: entity);

        // When
        FileContent result = orchestrationService.Get(id: id);

        // Then
        result.Should()
            .BeSameAs(expected: entity);

        fileContentProcessingServiceMock.Verify(expression: x => x.Get(id: id), times: Times.Once);
        fileContentProcessingServiceMock.VerifyNoOtherCalls();
        fileContentEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}