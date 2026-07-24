// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Brokers;
using cCoder.DocumentManagement.Brokers.Events;
using Moq;


namespace cCoder.Core.Services.Tests.DMS.Foundations.Events;

public partial class FileEventServiceTests
{
    private readonly Mock<IFileEventBroker> fileEventBrokerMock;
    private readonly Mock<IAuthInfoBroker> authInfoBrokerMock;
    private readonly cCoder.DocumentManagement.Services.Foundations.Events.FileEventService service;
    private const string CurrentUserId = "test-user";

    public FileEventServiceTests()
    {
        fileEventBrokerMock = new Mock<IFileEventBroker>(behavior: MockBehavior.Strict);
        authInfoBrokerMock = new Mock<IAuthInfoBroker>(behavior: MockBehavior.Strict);
        fileEventBrokerMock = new(behavior: MockBehavior.Strict);
        authInfoBrokerMock = new();
        authInfoBrokerMock.Setup(expression: x => x.GetCurrentSsoUserId())
            .Returns(value: CurrentUserId);
        service = new cCoder.DocumentManagement.Services.Foundations.Events.FileEventService(
            fileEventBroker: fileEventBrokerMock.Object,
            authInfoBroker: authInfoBrokerMock.Object
        );
    }
}