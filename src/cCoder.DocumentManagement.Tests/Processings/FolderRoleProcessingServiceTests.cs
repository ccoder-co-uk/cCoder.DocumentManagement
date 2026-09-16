// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Foundations;
using cCoder.DocumentManagement.Services.Processings;
using Moq;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FolderRoleProcessingServiceTests
{
    private User currentUser = ToLocalUser(user: TestUsers.WithoutPrivileges());
    private readonly Mock<IFolderRoleService> folderRoleServiceMock = new();
    private readonly FolderRoleProcessingService folderRoleProcessingService;

    public FolderRoleProcessingServiceTests()
    {
        folderRoleProcessingService = new FolderRoleProcessingService(
            service: folderRoleServiceMock.Object);
    }

    private static User ToLocalUser(cCoder.Data.Models.Security.User user) =>
        user == null
            ? null
            : new User
            {
                Id = user.Id,
                Roles = user.Roles?
                    .Select(selector: role => new UserRole
                    {
                        UserId = role.UserId,
                        RoleId = role.RoleId,
                        Role = role.Role == null
                            ? null
                            : new Role
                            {
                                Id = role.Role.Id,
                                AppId = role.Role.AppId,
                                Name = role.Role.Name,
                                Description = role.Role.Description,
                                Privs = role.Role.Privs,
                            },
                    })
                    .ToArray(),
            };

    private static Folder CreateFolder(
        params FolderRole[] folderRoles) =>
        new()
        {
            Id = Guid.NewGuid(),
            AppId = 1,
            Name = "Root",
            Path = "root",
            App = new App
            {
                Id = 1,
                Name = "App",
            },
            Roles = folderRoles,
            Files = [],
            SubFolders = [],
        };

    private static FolderRoleContext CreateFolderRoleContext(
        Folder folder,
        Role role) =>
        new()
        {
            Folder = folder,
            Role = role,
        };

    private void SetupFolderRoleContext(
        FolderRole folderRole,
        FolderRoleContext context)
    {
        folderRoleServiceMock
            .Setup(expression: service => service.CanCreateFolderRole(
                folderRole: It.Is<FolderRole>(match: item =>
                    item.FolderId == folderRole.FolderId
                    && item.RoleId == folderRole.RoleId)))
            .Returns(value: context.Role is not null
                && context.Folder is not null
                && context.Folder.UserCan(
                    user: currentUser,
                    privilege: "folderrole_create"));

        folderRoleServiceMock
            .Setup(expression: service => service.CanDeleteFolderRole(
                folderRole: It.Is<FolderRole>(match: item =>
                    item.FolderId == folderRole.FolderId
                    && item.RoleId == folderRole.RoleId)))
            .Returns(value: context.Folder is not null
                && context.Folder.UserCan(
                    user: currentUser,
                    privilege: "folderrole_delete"));
    }
}