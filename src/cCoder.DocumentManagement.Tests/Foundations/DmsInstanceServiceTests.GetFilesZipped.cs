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
    public void ShouldReturnBrokerResultWhenGetFilesZipped()
    {
        // Given
        DmsPath firstPath = CreatePath("folder/one.txt");
        DmsPath secondPath = CreatePath("folder/two.txt");
        DmsPath[] paths = [firstPath, secondPath];
        DMSResult result = CreateDmsResult("application/zip");
        dmsInstanceBrokerMock.Setup(x => x.GetFilesZipped(paths)).Returns(result);

        // When
        DMSResult returnedResult = dmsInstanceService.GetFilesZipped(paths);

        // Then
        returnedResult.Should().BeSameAs(result);
        dmsInstanceBrokerMock.Verify(x => x.GetFilesZipped(paths), Times.Once);
        dmsInstanceBrokerMock.VerifyNoOtherCalls();
    }

}




