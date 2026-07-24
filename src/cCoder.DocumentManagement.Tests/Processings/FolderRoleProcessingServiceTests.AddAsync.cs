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

        Folder folder = CreateFolder(
            folderRoles:
            [
                new FolderRole
                {
                    RoleId = currentUserRole.RoleId,
                    Role = currentUserRole.Role,
                },
            ]);

        FolderRole link = new() { FolderId = folder.Id, RoleId = roleToAdd.Id };
        currentUser = user;

        FolderRoleContext context = new()
        {
            Folder = folder,
            Role = roleToAdd,
        };

        contextBrokerMock
            .Setup(expression: broker =>
                broker.SelectFolderRoleContext(
                    folderRole: link,
                    ignoreFilters: true))
            .Returns(value: context);

        folderRoleServiceMock.Setup(expression: x => x.AddFolderRoleAsync(newFolderRole: link))
            .ReturnsAsync(value: link);

        // When
        FolderRole result = await folderRoleProcessingService.AddFolderRoleAsync(newFolderRole: link);

        // Then
        Assert.Same(expected: link, actual: result);
        folderRoleServiceMock.Verify(expression: x => x.AddFolderRoleAsync(newFolderRole: link), times: Times.Once);
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
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

        Folder folder = CreateFolder(
            folderRoles: []);

        FolderRole link = new() { FolderId = folder.Id, RoleId = roleToAdd.Id };

        FolderRoleContext context = new()
        {
            Folder = folder,
            Role = roleToAdd,
        };

        contextBrokerMock
            .Setup(expression: broker =>
                broker.SelectFolderRoleContext(
                    folderRole: link,
                    ignoreFilters: true))
            .Returns(value: context);

        // When
        Func<Task> action = async () =>
            await folderRoleProcessingService.AddFolderRoleAsync(newFolderRole: link);

        // Then
        await action.Should()
            .ThrowAsync<DocumentManagementServiceException>()
            .WithInnerException(innerException: typeof(SecurityException));
    }

}