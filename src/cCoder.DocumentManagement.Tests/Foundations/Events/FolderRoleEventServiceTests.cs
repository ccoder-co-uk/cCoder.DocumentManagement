// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        folderRoleEventBrokerMock = new Mock<IFolderRoleEventBroker>(behavior: MockBehavior.Strict);
        authInfoMock = new Mock<ICoreAuthInfo>(behavior: MockBehavior.Strict);
        folderRoleEventBrokerMock = new(behavior: MockBehavior.Strict);
        authInfoMock = new();
        authInfoMock.SetupGet(expression: x => x.SSOUserId)
            .Returns(value: CurrentUserId);
        service = new cCoder.DocumentManagement.Services.Foundations.Events.FolderRoleEventService(
            folderRoleEventBroker: folderRoleEventBrokerMock.Object,
            authInfo: authInfoMock.Object
        );
    }
}