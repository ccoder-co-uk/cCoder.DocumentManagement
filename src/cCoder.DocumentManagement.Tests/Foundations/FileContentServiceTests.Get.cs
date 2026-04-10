using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Foundations;

public partial class FileContentServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGet()
    {
        // Given
        Guid fileContentId = Guid.NewGuid();
        FileContent fileContent = CreateRandomFileContent(id: fileContentId);

        fileContentBrokerMock
            .Setup(x => x.GetAllFileContents(false))
            .Returns(new[] { ToExternalFileContent(fileContent) }.AsQueryable());

        // When
        FileContent result = fileContentService.Get(fileContentId);

        // Then
        result.Should().BeEquivalentTo(fileContent);
        fileContentBrokerMock.Verify(x => x.GetAllFileContents(false), Times.Once);
        fileContentBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}







