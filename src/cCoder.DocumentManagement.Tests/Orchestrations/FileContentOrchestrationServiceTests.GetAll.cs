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
    public void ShouldReturnProcessingResultsWhenGetAll()
    {
        // Given
        IQueryable<FileContent> entities = new[] { CreateRandomFileContent() }.AsQueryable();
        fileContentProcessingServiceMock.Setup(x => x.GetAll(true)).Returns(entities);

        // When
        IQueryable<FileContent> result = orchestrationService.GetAll(true);

        // Then
        result.Should().BeSameAs(entities);
        fileContentProcessingServiceMock.Verify(x => x.GetAll(true), Times.Once);
        fileContentProcessingServiceMock.VerifyNoOtherCalls();
        fileContentEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}








