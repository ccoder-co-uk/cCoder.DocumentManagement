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
    public async Task ShouldUseDataContextWhenUserCanCreateFolderRoleForAddAsync()
    {
        // Given
        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(It.IsAny<int?>(), It.IsAny<string>()))
            .Callback(action: (int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId, privilege) ?? false))
                {
                    throw new SecurityException("Access Denied!");
                }
            });

        authorizationBrokerMock
            .Setup(expression: x => x.IsAdminOfApp(It.IsAny<int>()))
            .Returns(valueFunction: (int appId) => currentUser?.IsAdminOfApp(appId) ?? false);

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser()).Returns(valueFunction: () => currentUser);

        User user = ToLocalUser(user: TestUsers.WithPrivilege("folderrole_create", 1));
        UserRole currentUserRole = user.Roles.First();
        DataRole roleToAdd = new()
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
        FolderRole link = new() { FolderId = folder.Id, RoleId = roleToAdd.Id };
        currentUser = user;
        roleBrokerMock.Setup(expression: x => x.GetAllRoles(true)).Returns(value: new[] { roleToAdd }.AsQueryable());
        folderServiceMock.Setup(expression: x => x.GetAll(true)).Returns(value: new[] { folder }.AsQueryable());
        folderRoleServiceMock.Setup(expression: x => x.AddAsync(link)).ReturnsAsync(value: link);

        // When
        FolderRole result = await folderRoleProcessingService.AddAsync(entity: link);

        // Then
        Assert.Same(expected: link, actual: result);
        folderRoleServiceMock.Verify(expression: x => x.AddAsync(link), times: Times.Once);
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(It.IsAny<int?>(), It.IsAny<string>()))
            .Callback(action: (int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId, privilege) ?? false))
                {
                    throw new SecurityException("Access Denied!");
                }
            });

        authorizationBrokerMock
            .Setup(expression: x => x.IsAdminOfApp(It.IsAny<int>()))
            .Returns(valueFunction: (int appId) => currentUser?.IsAdminOfApp(appId) ?? false);

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser()).Returns(valueFunction: () => currentUser);

        DataRole roleToAdd = new()
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
        FolderRole link = new() { FolderId = folder.Id, RoleId = roleToAdd.Id };
        roleBrokerMock.Setup(expression: x => x.GetAllRoles(true)).Returns(value: new[] { roleToAdd }.AsQueryable());
        folderServiceMock.Setup(expression: x => x.GetAll(true)).Returns(value: new[] { folder }.AsQueryable());

        // When
        await Assert.ThrowsAsync<SecurityException>(testCode: async () =>
            await folderRoleProcessingService.AddAsync(link)
        );

        // Then
    }

}