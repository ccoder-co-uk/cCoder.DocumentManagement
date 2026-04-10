using cCoder.Data;
using cCoder.DocumentManagement.Brokers.Events;
using Moq;


namespace cCoder.Core.Services.Tests.DMS.Foundations.Events;

public partial class FileContentEventServiceTests
{
    private readonly Mock<IFileContentEventBroker> fileContentEventBrokerMock;
    private readonly Mock<ICoreAuthInfo> authInfoMock;
    private readonly cCoder.DocumentManagement.Services.Foundations.Events.FileContentEventService service;
    private const string CurrentUserId = "test-user";

    public FileContentEventServiceTests()
    {
        fileContentEventBrokerMock = new Mock<IFileContentEventBroker>(MockBehavior.Strict);
        authInfoMock = new Mock<ICoreAuthInfo>(MockBehavior.Strict);
        fileContentEventBrokerMock = new(MockBehavior.Strict);
        authInfoMock = new();
        authInfoMock.SetupGet(x => x.SSOUserId).Returns(CurrentUserId);
        service = new cCoder.DocumentManagement.Services.Foundations.Events.FileContentEventService(
            fileContentEventBrokerMock.Object,
            authInfoMock.Object
        );
    }
}









