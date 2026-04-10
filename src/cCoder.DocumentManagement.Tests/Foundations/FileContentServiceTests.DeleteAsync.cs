using System.Security;
using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;
using DataFileContent = cCoder.Data.Models.DMS.FileContent;


namespace cCoder.Core.Services.Tests.DMS.Foundations;

public partial class FileContentServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForDeleteAsync()
    {
        // Given
        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(new User { Id = "test-user" });
        Guid fileContentId = Guid.NewGuid();
        FileContent fileContent = CreateRandomFileContent(id: fileContentId);

        fileContentBrokerMock
            .Setup(x => x.GetAllFileContents(false))
            .Returns(new[] { ToExternalFileContent(fileContent) }.AsQueryable());

        fileContentBrokerMock.Setup(x => x.GetAppId(It.IsAny<DataFileContent>())).Returns((int?)7);
        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "FileContent_delete"));
        fileContentBrokerMock
            .Setup(x => x.DeleteFileContentAsync(It.IsAny<DataFileContent>()))
            .ReturnsAsync(1);

        // When
        await fileContentService.DeleteAsync(fileContentId);

        // Then
        fileContentBrokerMock.Verify(x => x.GetAllFileContents(false), Times.Once);
        fileContentBrokerMock.Verify(
            x => x.DeleteFileContentAsync(It.Is<DataFileContent>(candidate => candidate.Id == fileContent.Id)),
            Times.Once
        );
        fileContentBrokerMock.Verify(
            x => x.GetAppId(It.IsAny<DataFileContent>()),
            Times.AtMostOnce()
        );
        fileContentBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "FileContent_delete"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        Guid fileContentId = Guid.NewGuid();
        FileContent fileContent = CreateRandomFileContent(id: fileContentId);

        fileContentBrokerMock
            .Setup(x => x.GetAllFileContents(false))
            .Returns(new[] { ToExternalFileContent(fileContent) }.AsQueryable());

        fileContentBrokerMock.Setup(x => x.GetAppId(It.IsAny<DataFileContent>())).Returns((int?)7);
        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "FileContent_delete"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await fileContentService.DeleteAsync(fileContentId);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        fileContentBrokerMock.Verify(x => x.GetAllFileContents(false), Times.Once);
        fileContentBrokerMock.Verify(
            x => x.GetAppId(It.IsAny<DataFileContent>()),
            Times.AtMostOnce()
        );
        fileContentBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "FileContent_delete"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}







