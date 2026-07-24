// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Brokers;
using cCoder.DocumentManagement.Brokers.Events;
using Moq;


namespace cCoder.Core.Services.Tests.DMS.Foundations.Events;

public partial class FolderRoleEventServiceTests
{
    private readonly Mock<IFolderRoleEventBroker> folderRoleEventBrokerMock;
    private readonly Mock<IAuthInfoBroker> authInfoBrokerMock;
    private readonly cCoder.DocumentManagement.Services.Foundations.Events.FolderRoleEventService service;
    private const string CurrentUserId = "test-user";

    public FolderRoleEventServiceTests()
    {
        folderRoleEventBrokerMock = new Mock<IFolderRoleEventBroker>(behavior: MockBehavior.Strict);
        authInfoBrokerMock = new Mock<IAuthInfoBroker>(behavior: MockBehavior.Strict);
        folderRoleEventBrokerMock = new(behavior: MockBehavior.Strict);
        authInfoBrokerMock = new();
        authInfoBrokerMock.Setup(expression: x => x.GetCurrentSsoUserId())
            .Returns(value: CurrentUserId);
        service = new cCoder.DocumentManagement.Services.Foundations.Events.FolderRoleEventService(
            folderRoleEventBroker: folderRoleEventBrokerMock.Object,
            authInfoBroker: authInfoBrokerMock.Object
        );
    }
}