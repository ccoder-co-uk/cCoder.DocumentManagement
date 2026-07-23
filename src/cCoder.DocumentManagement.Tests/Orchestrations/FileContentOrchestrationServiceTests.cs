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

public partial class FileContentOrchestrationServiceTests
{
    private readonly Mock<IFileContentProcessingService> fileContentProcessingServiceMock;
    private readonly Mock<IFileContentEventProcessingService> fileContentEventProcessingServiceMock;
    private readonly FileContentOrchestrationService orchestrationService;

    public FileContentOrchestrationServiceTests()
    {
        fileContentProcessingServiceMock = new Mock<IFileContentProcessingService>(behavior: MockBehavior.Strict);
        fileContentEventProcessingServiceMock = new Mock<IFileContentEventProcessingService>(behavior: MockBehavior.Strict);
        orchestrationService = new FileContentOrchestrationService(
            processingService: fileContentProcessingServiceMock.Object,
            eventService: fileContentEventProcessingServiceMock.Object
        );
    }

    private static FileContent CreateRandomFileContent() =>
        Builder<FileContent>.CreateNew().Build();
}