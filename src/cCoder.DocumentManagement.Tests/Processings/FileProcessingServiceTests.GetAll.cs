using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FileProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGetAll()
    {
        // Given
        IQueryable<cCoder.Data.Models.DMS.File> files = new[]
        {
            CreateRandomFile(),
        }.AsQueryable();
        fileServiceMock.Setup(x => x.GetAll()).Returns(files);

        // When
        IQueryable<cCoder.Data.Models.DMS.File> result = fileProcessingService.GetAll();

        // Then
        result.Should().BeSameAs(files);
        fileServiceMock.Verify(x => x.GetAll(), Times.Once);
        fileServiceMock.VerifyNoOtherCalls();
    }

}







