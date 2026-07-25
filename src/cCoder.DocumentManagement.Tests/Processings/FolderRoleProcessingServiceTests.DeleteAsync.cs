// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
            .Setup(expression: x => x.Authorize(appId: It.IsAny<int?>(), privilege: It.IsAny<string>()))
            .Callback(action: (int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId: appId, operation: privilege) ?? false))
                {
                    throw new SecurityException(message: "Access Denied!");
                }
            });

        authorizationBrokerMock
            .Setup(expression: x => x.IsAdminOfApp(appId: It.IsAny<int>()))
            .Returns(valueFunction: (int appId) => currentUser?.IsAdminOfApp(appId: appId) ?? false);

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(valueFunction: () => currentUser);

        User user = ToLocalUser(user: TestUsers.WithPrivilege(privilege: "folderrole_delete", appId: 1));
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

        contextBrokerMock
            .Setup(expression: broker =>
                broker.SelectFolderRoleContext(
                    folderRole: It.Is<FolderRole>(
                        match: item =>
                            item.FolderId == folder.Id
                            && item.RoleId == role.Id),
                    ignoreFilters: true))
            .Returns(value: new FolderRoleContext
            {
                Folder = folder,
                Role = role,
            });

        folderRoleServiceMock.Setup(expression: x => x.GetAll(ignoreFilters: true))
            .Returns(value: new[] { link }.AsQueryable());

        folderRoleServiceMock.Setup(expression: x => x.DeleteFolderRoleAsync(deletedFolderRole: link))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await folderRoleProcessingService.DeleteFolderRoleAsync(
            deletedFolderRole: new FolderRole { FolderId = folder.Id, RoleId = role.Id }
        );

        // Then
        contextBrokerMock.Verify(
            expression: broker =>
                broker.SelectFolderRoleContext(
                    folderRole: It.Is<FolderRole>(
                        match: item =>
                            item.FolderId == folder.Id
                            && item.RoleId == role.Id),
                    ignoreFilters: true),
            times: Times.Once);

        folderRoleServiceMock.Verify(expression: x => x.GetAll(ignoreFilters: true), times: Times.Once);

        folderRoleServiceMock.Verify(
            expression: x =>
                x.DeleteFolderRoleAsync(
                    deletedFolderRole: It.Is<FolderRole>(match: item => item.RoleId == role.Id && item.FolderId == folder.Id)
                ),
            times: Times.Once
        );
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: It.IsAny<int?>(), privilege: It.IsAny<string>()))
            .Callback(action: (int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId: appId, operation: privilege) ?? false))
                {
                    throw new SecurityException(message: "Access Denied!");
                }
            });

        authorizationBrokerMock
            .Setup(expression: x => x.IsAdminOfApp(appId: It.IsAny<int>()))
            .Returns(valueFunction: (int appId) => currentUser?.IsAdminOfApp(appId: appId) ?? false);

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(valueFunction: () => currentUser);

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

        contextBrokerMock
            .Setup(expression: broker =>
                broker.SelectFolderRoleContext(
                    folderRole: It.Is<FolderRole>(
                        match: item =>
                            item.FolderId == folder.Id
                            && item.RoleId == role.Id),
                    ignoreFilters: true))
            .Returns(value: new FolderRoleContext
            {
                Folder = folder,
                Role = role,
            });

        folderRoleServiceMock.Setup(expression: x => x.GetAll(ignoreFilters: true))
            .Returns(value: new[] { link }.AsQueryable());

        // When
        Func<Task> action = async () =>
            await folderRoleProcessingService.DeleteFolderRoleAsync(
                deletedFolderRole: new FolderRole { FolderId = folder.Id, RoleId = role.Id }
            );

        // Then
        await action.Should()
            .ThrowAsync<DocumentManagementServiceException>()
            .WithInnerException(innerException: typeof(SecurityException));
    }

}