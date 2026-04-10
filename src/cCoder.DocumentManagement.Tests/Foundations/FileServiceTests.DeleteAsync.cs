using System.Security;
using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;
using DataFile = cCoder.Data.Models.DMS.File;
using FileEntity = cCoder.Data.Models.DMS.File;


namespace cCoder.Core.Services.Tests.DMS.Foundations;

public partial class FileServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForDeleteAsync()
    {
        // Given
        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(new User { Id = "test-user" });
        Guid fileId = Guid.NewGuid();
        FileEntity file = CreateRandomFile(id: fileId);

        fileBrokerMock
            .Setup(x => x.GetAllFiles(true))
            .Returns(new[] { ToExternalFile(file) }.AsQueryable());

        fileBrokerMock.Setup(x => x.GetAppId(It.IsAny<DataFile>())).Returns((int?)7);
        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "file_delete"));
        fileBrokerMock.Setup(x => x.DeleteFileAsync(It.IsAny<DataFile>())).ReturnsAsync(1);

        // When
        await fileService.DeleteAsync(fileId);

        // Then
        fileBrokerMock.Verify(x => x.GetAllFiles(true), Times.Once);
        fileBrokerMock.Verify(
            x => x.DeleteFileAsync(It.Is<DataFile>(candidate => candidate.Id == file.Id)),
            Times.Once
        );
        fileBrokerMock.Verify(x => x.GetAppId(It.IsAny<DataFile>()), Times.AtMostOnce());
        fileBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "file_delete"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        Guid fileId = Guid.NewGuid();
        FileEntity file = CreateRandomFile(id: fileId);

        fileBrokerMock
            .Setup(x => x.GetAllFiles(true))
            .Returns(new[] { ToExternalFile(file) }.AsQueryable());

        fileBrokerMock.Setup(x => x.GetAppId(It.IsAny<DataFile>())).Returns((int?)7);
        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "file_delete"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await fileService.DeleteAsync(fileId);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        fileBrokerMock.Verify(x => x.GetAllFiles(true), Times.Once);
        fileBrokerMock.Verify(x => x.GetAppId(It.IsAny<DataFile>()), Times.AtMostOnce());
        fileBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "file_delete"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}








