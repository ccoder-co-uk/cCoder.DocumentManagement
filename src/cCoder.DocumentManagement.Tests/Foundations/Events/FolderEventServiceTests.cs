// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        folderEventBrokerMock = new Mock<IFolderEventBroker>(behavior: MockBehavior.Strict);
        authInfoMock = new Mock<ICoreAuthInfo>(behavior: MockBehavior.Strict);
        folderEventBrokerMock = new(behavior: MockBehavior.Strict);
        authInfoMock = new();
        authInfoMock.SetupGet(expression: x => x.SSOUserId)
            .Returns(value: CurrentUserId);
        service = new cCoder.DocumentManagement.Services.Foundations.Events.FolderEventService(
            folderEventBroker: folderEventBrokerMock.Object,
            authInfo: authInfoMock.Object
        );
    }
}