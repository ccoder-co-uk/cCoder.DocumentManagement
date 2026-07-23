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

        User user = ToLocalUser(user: TestUsers.WithPrivilege(privilege: "folderrole_create", appId: 1));
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

        roleBrokerMock.Setup(expression: x => x.GetAllRoles(ignoreFilters: true))
            .Returns(value: new[] { roleToAdd }.AsQueryable());

        folderServiceMock.Setup(expression: x => x.GetAll(ignoreFilters: true))
            .Returns(value: new[] { folder }.AsQueryable());

        folderRoleServiceMock.Setup(expression: x => x.AddAsync(folderRole: link))
            .ReturnsAsync(value: link);

        // When
        FolderRole result = await folderRoleProcessingService.AddAsync(entity: link);

        // Then
        Assert.Same(expected: link, actual: result);
        folderRoleServiceMock.Verify(expression: x => x.AddAsync(folderRole: link), times: Times.Once);
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
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

        roleBrokerMock.Setup(expression: x => x.GetAllRoles(ignoreFilters: true))
            .Returns(value: new[] { roleToAdd }.AsQueryable());

        folderServiceMock.Setup(expression: x => x.GetAll(ignoreFilters: true))
            .Returns(value: new[] { folder }.AsQueryable());

        // When
        await Assert.ThrowsAsync<SecurityException>(testCode: async () =>
            await folderRoleProcessingService.AddAsync(entity: link)
        );

        // Then
    }

}