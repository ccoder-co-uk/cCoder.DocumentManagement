using System.Security;
using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;
using DataFolder = cCoder.Data.Models.DMS.Folder;


namespace cCoder.Core.Services.Tests.DMS.Foundations;

public partial class FolderServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForDeleteAsync()
    {
        // Given
        Guid folderId = Guid.NewGuid();
        Folder folder = CreateRandomFolder(id: folderId, appId: 7);

        folderBrokerMock
            .Setup(x => x.GetAllFolders(true))
            .Returns(new[] { ToExternalFolder(folder) }.AsQueryable());

        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "Folder_delete"));
        folderBrokerMock.Setup(x => x.DeleteFolderAsync(It.IsAny<DataFolder>())).ReturnsAsync(1);

        // When
        await folderService.DeleteAsync(folderId);

        // Then
        folderBrokerMock.Verify(x => x.GetAllFolders(true), Times.Once);
        folderBrokerMock.Verify(
            x => x.DeleteFolderAsync(It.Is<DataFolder>(candidate => candidate.Id == folder.Id)),
            Times.Once
        );
        folderBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "Folder_delete"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        Guid folderId = Guid.NewGuid();
        Folder folder = CreateRandomFolder(id: folderId, appId: 7);

        folderBrokerMock
            .Setup(x => x.GetAllFolders(true))
            .Returns(new[] { ToExternalFolder(folder) }.AsQueryable());

        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "Folder_delete"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await folderService.DeleteAsync(folderId);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        folderBrokerMock.Verify(x => x.GetAllFolders(true), Times.Once);
        folderBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "Folder_delete"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}







