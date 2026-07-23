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

public partial class FolderOrchestrationServiceTests
{
    private readonly Mock<IFolderProcessingService> folderProcessingServiceMock;
    private readonly Mock<IFolderEventProcessingService> folderEventProcessingServiceMock;
    private readonly FolderOrchestrationService orchestrationService;

    public FolderOrchestrationServiceTests()
    {
        folderProcessingServiceMock = new Mock<IFolderProcessingService>(behavior: MockBehavior.Strict);
        folderEventProcessingServiceMock = new Mock<IFolderEventProcessingService>(behavior: MockBehavior.Strict);
        orchestrationService = new FolderOrchestrationService(
            processingService: folderProcessingServiceMock.Object,
            eventService: folderEventProcessingServiceMock.Object
        );
    }

    private static Folder CreateRandomFolder() =>
        Builder<Folder>.CreateNew()
                                                                             .Build();
}