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
using IRoleBroker = cCoder.DocumentManagement.Brokers.IRoleBroker;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FolderRoleProcessingServiceTests
{
    private readonly Mock<IFolderService> folderServiceMock = new();
    private User currentUser = ToLocalUser(user: TestUsers.WithoutPrivileges());
    private readonly Mock<IFolderRoleService> folderRoleServiceMock = new();
    private readonly Mock<IRoleBroker> roleBrokerMock = new();
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock = new();
    private readonly FolderRoleProcessingService folderRoleProcessingService;

    public FolderRoleProcessingServiceTests()
    {
        folderRoleProcessingService = new FolderRoleProcessingService(
            service: folderRoleServiceMock.Object,
            roleBroker: roleBrokerMock.Object,
            folderService: folderServiceMock.Object,
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
}