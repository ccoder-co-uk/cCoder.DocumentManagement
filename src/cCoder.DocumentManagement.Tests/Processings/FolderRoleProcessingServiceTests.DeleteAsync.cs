using System.Security;
using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;
using DataRole = cCoder.Data.Models.Security.Role;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FolderRoleProcessingServiceTests
{
    [Fact]
    public async Task ShouldUseFoundationDeleteWhenUserCanDeleteFolderRoleForDeleteAsync()
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

        User user = ToLocalUser(TestUsers.WithPrivilege("folderrole_delete", 1));
        UserRole currentUserRole = user.Roles.First();
        DataRole role = new()
        {
            Id = Guid.NewGuid(),
            AppId = 1,
            Name = "Editors",
            Privs = "folder_read",
            Users = [],
            Folders = [],
        };
        Folder folder = new()
        {
            Id = Guid.NewGuid(),
            AppId = 1,
            Name = "Root",
            Path = "root",
            App = new App { Id = 1, Name = "App" },
            Roles =
            [
                new FolderRole { RoleId = currentUserRole.RoleId, Role = currentUserRole.Role },
            ],
            Files = [],
            SubFolders = [],
        };
        FolderRole link = new()
        {
            FolderId = folder.Id,
            RoleId = role.Id,
        };
        currentUser = user;
        folderServiceMock.Setup(x => x.GetAll(true)).Returns(new[] { folder }.AsQueryable());
        folderRoleServiceMock.Setup(x => x.GetAll(true)).Returns(new[] { link }.AsQueryable());
        folderRoleServiceMock.Setup(x => x.DeleteAsync(link)).Returns(ValueTask.CompletedTask);

        // When
        await folderRoleProcessingService.DeleteAsync(
            new FolderRole { FolderId = folder.Id, RoleId = role.Id }
        );

        // Then
        folderServiceMock.Verify(x => x.GetAll(true), Times.Once);
        folderRoleServiceMock.Verify(x => x.GetAll(true), Times.Once);
        folderRoleServiceMock.Verify(
            x =>
                x.DeleteAsync(
                    It.Is<FolderRole>(item => item.RoleId == role.Id && item.FolderId == folder.Id)
                ),
            Times.Once
        );
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

        DataRole role = new()
        {
            Id = Guid.NewGuid(),
            AppId = 1,
            Name = "Editors",
            Privs = "folder_read",
            Users = [],
            Folders = [],
        };
        Folder folder = new()
        {
            Id = Guid.NewGuid(),
            AppId = 1,
            Name = "Root",
            Path = "root",
            App = new App { Id = 1, Name = "App" },
            Roles = [],
            Files = [],
            SubFolders = [],
        };
        FolderRole link = new()
        {
            FolderId = folder.Id,
            RoleId = role.Id,
        };
        folderServiceMock.Setup(x => x.GetAll(true)).Returns(new[] { folder }.AsQueryable());
        folderRoleServiceMock.Setup(x => x.GetAll(true)).Returns(new[] { link }.AsQueryable());

        // When
        await Assert.ThrowsAsync<SecurityException>(async () =>
            await folderRoleProcessingService.DeleteAsync(
                new FolderRole { FolderId = folder.Id, RoleId = role.Id }
            )
        );

        // Then
    }

}












