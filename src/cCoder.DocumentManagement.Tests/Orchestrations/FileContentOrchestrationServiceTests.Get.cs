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
        fileContentProcessingServiceMock.Setup(x => x.Get(id)).Returns(entity);

        // When
        FileContent result = orchestrationService.Get(id);

        // Then
        result.Should().BeSameAs(entity);
        fileContentProcessingServiceMock.Verify(x => x.Get(id), Times.Once);
        fileContentProcessingServiceMock.VerifyNoOtherCalls();
        fileContentEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}








