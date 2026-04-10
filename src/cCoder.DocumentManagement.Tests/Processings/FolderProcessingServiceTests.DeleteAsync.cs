using System.Security;
using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FolderProcessingServiceTests
{
    [Fact]
    public async Task ShouldDeleteFolderWhenUserIsAppAdminForDeleteAsync()
    {
        // Given
        authorizationBrokerMock
            .Setup(x => x.Authorize(It.IsAny<int?>(), It.IsAny<string>()))
            .Callback((int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId, privilege) ?? false))
                    throw new SecurityException("Access Denied!");
            });

        authorizationBrokerMock
            .Setup(x => x.IsAdminOfApp(It.IsAny<int>()))
            .Returns((int appId) => currentUser?.IsAdminOfApp(appId) ?? false);

        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(() => currentUser);

        User user = ToLocalUser(TestUsers.WithPrivilege("app_admin", 1));
        currentUser = user;
        Folder folder = CreateRandomFolder();
        folderServiceMock.Setup(x => x.GetWithRoles(folder.Id, true)).Returns(folder);
        folderServiceMock.Setup(x => x.DeleteAsync(folder.Id)).Returns(ValueTask.CompletedTask);

        // When
        await folderProcessingService.DeleteAsync(folder.Id);

        // Then
        folderServiceMock.Verify(x => x.GetWithRoles(folder.Id, true), Times.Once);
        folderServiceMock.Verify(x => x.DeleteAsync(folder.Id), Times.Once);
        folderServiceMock.VerifyNoOtherCalls();
        loggerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.GetCurrentUser(), Times.AtLeastOnce);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        authorizationBrokerMock
            .Setup(x => x.Authorize(It.IsAny<int?>(), It.IsAny<string>()))
            .Callback((int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId, privilege) ?? false))
                    throw new SecurityException("Access Denied!");
            });

        authorizationBrokerMock
            .Setup(x => x.IsAdminOfApp(It.IsAny<int>()))
            .Returns((int appId) => currentUser?.IsAdminOfApp(appId) ?? false);

        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(() => currentUser);

        Folder folder = CreateRandomFolder();
        currentUser = ToLocalUser(TestUsers.WithoutPrivileges());
        folderServiceMock.Setup(x => x.GetWithRoles(folder.Id, true)).Returns(folder);

        // When
        Func<Task> act = async () => await folderProcessingService.DeleteAsync(folder.Id);

        // Then
        await act.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        folderServiceMock.Verify(x => x.GetWithRoles(folder.Id, true), Times.Once);
        folderServiceMock.VerifyNoOtherCalls();
        loggerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.GetCurrentUser(), Times.AtLeastOnce);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}











