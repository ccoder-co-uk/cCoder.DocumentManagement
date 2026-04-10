using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FileContentProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGet()
    {
        // Given
        FileContent entity = CreateRandomFileContent();
        var id = entity.Id;
        fileContentServiceMock.Setup(x => x.Get(id)).Returns(entity);

        // When
        FileContent result = fileContentProcessingService.Get(id);

        // Then
        result.Should().BeSameAs(entity);
        fileContentServiceMock.Verify(x => x.Get(id), Times.Once);
        fileContentServiceMock.VerifyNoOtherCalls();
    }

}








