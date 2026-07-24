// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Brokers;
using cCoder.DocumentManagement.Brokers.Events;
using Moq;


namespace cCoder.Core.Services.Tests.DMS.Foundations.Events;

public partial class FileContentEventServiceTests
{
    private readonly Mock<IFileContentEventBroker> fileContentEventBrokerMock;
    private readonly Mock<IAuthInfoBroker> authInfoBrokerMock;
    private readonly cCoder.DocumentManagement.Services.Foundations.Events.FileContentEventService service;
    private const string CurrentUserId = "test-user";

    public FileContentEventServiceTests()
    {
        fileContentEventBrokerMock = new Mock<IFileContentEventBroker>(behavior: MockBehavior.Strict);
        authInfoBrokerMock = new Mock<IAuthInfoBroker>(behavior: MockBehavior.Strict);
        fileContentEventBrokerMock = new(behavior: MockBehavior.Strict);
        authInfoBrokerMock = new();
        authInfoBrokerMock.Setup(expression: x => x.GetCurrentSsoUserId())
            .Returns(value: CurrentUserId);
        service = new cCoder.DocumentManagement.Services.Foundations.Events.FileContentEventService(
            fileContentEventBroker: fileContentEventBrokerMock.Object,
            authInfoBroker: authInfoBrokerMock.Object
        );
    }
}