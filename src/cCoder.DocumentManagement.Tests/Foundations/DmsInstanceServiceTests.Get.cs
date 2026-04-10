using cCoder.Data;
using FluentAssertions;
using Moq;
using Xunit;
using DMSResult = cCoder.DocumentManagement.Models.DMSResult;
using DmsPath = cCoder.DocumentManagement.Models.Path;


namespace cCoder.Core.Services.Tests.DMS.Foundations;

public partial class DmsInstanceServiceTests
{
    [Fact]
    public void ShouldReturnBrokerResultWhenGet()
    {
        // Given
        DmsPath path = CreatePath("folder/file.txt");
        DMSResult result = CreateDmsResult("text/plain");
        dmsInstanceBrokerMock.Setup(x => x.Get(path, 3, "needle")).Returns(result);

        // When
        DMSResult returnedResult = dmsInstanceService.Get(path, 3, "needle");

        // Then
        returnedResult.Should().BeSameAs(result);
        dmsInstanceBrokerMock.Verify(x => x.Get(path, 3, "needle"), Times.Once);
        dmsInstanceBrokerMock.VerifyNoOtherCalls();
    }

}




