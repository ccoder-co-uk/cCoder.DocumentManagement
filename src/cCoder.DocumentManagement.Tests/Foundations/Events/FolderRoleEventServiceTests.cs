using cCoder.Data;
using cCoder.DocumentManagement.Brokers.Events;
using Moq;


namespace cCoder.Core.Services.Tests.DMS.Foundations.Events;

public partial class FolderRoleEventServiceTests
{
    private readonly Mock<IFolderRoleEventBroker> folderRoleEventBrokerMock;
    private readonly Mock<ICoreAuthInfo> authInfoMock;
    private readonly cCoder.DocumentManagement.Services.Foundations.Events.FolderRoleEventService service;
    private const string CurrentUserId = "test-user";

    public FolderRoleEventServiceTests()
    {
        folderRoleEventBrokerMock = new Mock<IFolderRoleEventBroker>(MockBehavior.Strict);
        authInfoMock = new Mock<ICoreAuthInfo>(MockBehavior.Strict);
        folderRoleEventBrokerMock = new(MockBehavior.Strict);
        authInfoMock = new();
        authInfoMock.SetupGet(x => x.SSOUserId).Returns(CurrentUserId);
        service = new cCoder.DocumentManagement.Services.Foundations.Events.FolderRoleEventService(
            folderRoleEventBrokerMock.Object,
            authInfoMock.Object
        );
    }
}









