// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Orchestrations;
using cCoder.DocumentManagement.Services.Processings;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.DMS.Orchestrations;

public partial class FolderRoleOrchestrationServiceTests
{
    private readonly Mock<IFolderRoleProcessingService> folderRoleProcessingServiceMock;
    private readonly Mock<IFolderRoleEventProcessingService> folderRoleEventProcessingServiceMock;
    private readonly FolderRoleOrchestrationService orchestrationService;

    public FolderRoleOrchestrationServiceTests()
    {
        folderRoleProcessingServiceMock = new Mock<IFolderRoleProcessingService>(behavior: MockBehavior.Strict);
        folderRoleEventProcessingServiceMock = new Mock<IFolderRoleEventProcessingService>(behavior: MockBehavior.Strict);
        orchestrationService = new FolderRoleOrchestrationService(
            processingService: folderRoleProcessingServiceMock.Object,
            eventService: folderRoleEventProcessingServiceMock.Object
        );
    }

    private static FolderRole CreateRandomFolderRole() => Builder<FolderRole>.CreateNew().Build();
}