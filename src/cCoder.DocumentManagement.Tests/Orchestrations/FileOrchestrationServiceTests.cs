// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Services.Orchestrations;
using cCoder.DocumentManagement.Services.Processings;
using Moq;


namespace cCoder.Core.Services.Tests.DMS.Orchestrations;

public partial class FileOrchestrationServiceTests
{
    private readonly Mock<IFileProcessingService> fileProcessingServiceMock;
    private readonly Mock<IFileEventProcessingService> fileEventProcessingServiceMock;
    private readonly FileOrchestrationService orchestrationService;

    public FileOrchestrationServiceTests()
    {
        fileProcessingServiceMock = new Mock<IFileProcessingService>(behavior: MockBehavior.Strict);
        fileEventProcessingServiceMock = new Mock<IFileEventProcessingService>(behavior: MockBehavior.Strict);
        orchestrationService = new FileOrchestrationService(
            processingService: fileProcessingServiceMock.Object,
            eventService: fileEventProcessingServiceMock.Object
        );
    }
}