using cCoder.Data;
using cCoder.DocumentManagement.Brokers.Events;
using Moq;


namespace cCoder.Core.Services.Tests.DMS.Foundations.Events;

public partial class FolderEventServiceTests
{
    private readonly Mock<IFolderEventBroker> folderEventBrokerMock;
    private readonly Mock<ICoreAuthInfo> authInfoMock;
    private readonly cCoder.DocumentManagement.Services.Foundations.Events.FolderEventService service;
    private const string CurrentUserId = "test-user";

    public FolderEventServiceTests()
    {
        folderEventBrokerMock = new Mock<IFolderEventBroker>(MockBehavior.Strict);
        authInfoMock = new Mock<ICoreAuthInfo>(MockBehavior.Strict);
        folderEventBrokerMock = new(MockBehavior.Strict);
        authInfoMock = new();
        authInfoMock.SetupGet(x => x.SSOUserId).Returns(CurrentUserId);
        service = new cCoder.DocumentManagement.Services.Foundations.Events.FolderEventService(
            folderEventBrokerMock.Object,
            authInfoMock.Object
        );
    }
}









