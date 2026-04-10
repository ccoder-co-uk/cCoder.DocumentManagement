using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Foundations;

public partial class DmsInstanceServiceTests
{
    [Fact]
    public void ShouldReturnBrokerResultsWhenSearch()
    {
        // Given
        cCoder.Data.Models.DMS.File firstFile = CreateFileAsync();
        cCoder.Data.Models.DMS.File secondFile = CreateFileAsync();
        cCoder.Data.Models.DMS.File[] files = [firstFile, secondFile];
        dmsInstanceBrokerMock.Setup(x => x.Search("term")).Returns(files);

        // When
        IEnumerable<cCoder.Data.Models.DMS.File> returnedFiles =
            dmsInstanceService.Search("term");

        // Then
        returnedFiles.Should().BeSameAs(files);
        dmsInstanceBrokerMock.Verify(x => x.Search("term"), Times.Once);
        dmsInstanceBrokerMock.VerifyNoOtherCalls();
    }

}






