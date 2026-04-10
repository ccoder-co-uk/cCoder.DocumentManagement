using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FileProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGet()
    {
        // Given
        cCoder.Data.Models.DMS.File file = CreateRandomFile();
        fileServiceMock.Setup(x => x.Get(file.Id)).Returns(file);

        // When
        cCoder.Data.Models.DMS.File result = fileProcessingService.Get(file.Id);

        // Then
        result.Should().BeSameAs(file);
        fileServiceMock.Verify(x => x.Get(file.Id), Times.Once);
        fileServiceMock.VerifyNoOtherCalls();
    }

}







