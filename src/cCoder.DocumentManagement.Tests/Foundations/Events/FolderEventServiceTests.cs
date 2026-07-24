// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Brokers;
using cCoder.DocumentManagement.Brokers.Events;
using Moq;


namespace cCoder.Core.Services.Tests.DMS.Foundations.Events;

public partial class FolderEventServiceTests
{
    private readonly Mock<IFolderEventBroker> folderEventBrokerMock;
    private readonly Mock<IAuthInfoBroker> authInfoBrokerMock;
    private readonly cCoder.DocumentManagement.Services.Foundations.Events.FolderEventService service;
    private const string CurrentUserId = "test-user";

    public FolderEventServiceTests()
    {
        folderEventBrokerMock = new Mock<IFolderEventBroker>(behavior: MockBehavior.Strict);
        authInfoBrokerMock = new Mock<IAuthInfoBroker>(behavior: MockBehavior.Strict);
        folderEventBrokerMock = new(behavior: MockBehavior.Strict);
        authInfoBrokerMock = new();
        authInfoBrokerMock.Setup(expression: x => x.GetCurrentSsoUserId())
            .Returns(value: CurrentUserId);
        service = new cCoder.DocumentManagement.Services.Foundations.Events.FolderEventService(
            folderEventBroker: folderEventBrokerMock.Object,
            authInfoBroker: authInfoBrokerMock.Object
        );
    }
}