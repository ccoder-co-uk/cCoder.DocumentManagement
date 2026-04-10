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
        fileProcessingServiceMock = new Mock<IFileProcessingService>(MockBehavior.Strict);
        fileEventProcessingServiceMock = new Mock<IFileEventProcessingService>(MockBehavior.Strict);
        orchestrationService = new FileOrchestrationService(
            fileProcessingServiceMock.Object,
            fileEventProcessingServiceMock.Object
        );
    }
}







