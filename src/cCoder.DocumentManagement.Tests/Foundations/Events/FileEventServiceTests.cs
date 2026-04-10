using cCoder.Data;
using cCoder.DocumentManagement.Brokers.Events;
using Moq;


namespace cCoder.Core.Services.Tests.DMS.Foundations.Events;

public partial class FileEventServiceTests
{
    private readonly Mock<IFileEventBroker> fileEventBrokerMock;
    private readonly Mock<ICoreAuthInfo> authInfoMock;
    private readonly cCoder.DocumentManagement.Services.Foundations.Events.FileEventService service;
    private const string CurrentUserId = "test-user";

    public FileEventServiceTests()
    {
        fileEventBrokerMock = new Mock<IFileEventBroker>(MockBehavior.Strict);
        authInfoMock = new Mock<ICoreAuthInfo>(MockBehavior.Strict);
        fileEventBrokerMock = new(MockBehavior.Strict);
        authInfoMock = new();
        authInfoMock.SetupGet(x => x.SSOUserId).Returns(CurrentUserId);
        service = new cCoder.DocumentManagement.Services.Foundations.Events.FileEventService(
            fileEventBrokerMock.Object,
            authInfoMock.Object
        );
    }
}









