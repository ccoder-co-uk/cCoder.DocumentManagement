using cCoder.DocumentManagement.Brokers.Storage;
using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Foundations;
using Moq;
using DataFolderRole = cCoder.Data.Models.Security.FolderRole;
using IAuthorizationBroker = cCoder.DocumentManagement.Brokers.IAuthorizationBroker;


namespace cCoder.Core.Services.Tests.DMS.Foundations;

public partial class FolderRoleServiceTests
{
    private readonly Mock<IFolderRoleBroker> folderRoleBrokerMock;
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock;
    private readonly FolderRoleService folderRoleService;

    public FolderRoleServiceTests()
    {
        folderRoleBrokerMock = new Mock<IFolderRoleBroker>(MockBehavior.Strict);
        authorizationBrokerMock = new Mock<IAuthorizationBroker>(MockBehavior.Strict);
        folderRoleService = new FolderRoleService(
            folderRoleBrokerMock.Object,
            authorizationBrokerMock.Object
        );
    }

    private static FolderRole CreateRandomFolderRole(Guid folderId = default, Guid roleId = default)
    {
        FolderRole folderRole = new()
        {
            FolderId = folderId == Guid.Empty ? Guid.NewGuid() : folderId,
            RoleId = roleId == Guid.Empty ? Guid.NewGuid() : roleId,
            Folder = null!,
            Role = null!,
        };

        return folderRole;
    }

    private static DataFolderRole ToExternalFolderRole(FolderRole folderRole) =>
        folderRole == null
            ? null
            : new DataFolderRole
            {
                FolderId = folderRole.FolderId,
                RoleId = folderRole.RoleId,
            };
}














