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
using IAuthorizationBroker = cCoder.DocumentManagement.Brokers.IAuthorizationBroker;
using IFolderRoleContextBroker = cCoder.DocumentManagement.Brokers.IFolderRoleContextBroker;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FolderRoleProcessingServiceTests
{
    private User currentUser = ToLocalUser(user: TestUsers.WithoutPrivileges());
    private readonly Mock<IFolderRoleService> folderRoleServiceMock = new();
    private readonly Mock<IFolderRoleContextBroker> contextBrokerMock = new();
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock = new();
    private readonly FolderRoleProcessingService folderRoleProcessingService;

    public FolderRoleProcessingServiceTests()
    {
        contextBrokerMock
            .Setup(expression: broker =>
                broker.SelectFolderRoleContext(
                    folderRole: It.IsAny<FolderRole>(),
                    ignoreFilters: It.IsAny<bool>()))
            .Returns(value: new FolderRoleContext());

        folderRoleProcessingService = new FolderRoleProcessingService(
            service: folderRoleServiceMock.Object,
            contextBroker: contextBrokerMock.Object,
            authorizationBroker: authorizationBrokerMock.Object
        );
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
        FolderRoleContext context) =>
        contextBrokerMock
            .Setup(expression: broker =>
                broker.SelectFolderRoleContext(
                    folderRole: folderRole,
                    ignoreFilters: true))
            .Returns(value: context);
}