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
        folderRoleProcessingServiceMock = new Mock<IFolderRoleProcessingService>(MockBehavior.Strict);
        folderRoleEventProcessingServiceMock = new Mock<IFolderRoleEventProcessingService>(MockBehavior.Strict);
        orchestrationService = new FolderRoleOrchestrationService(
            folderRoleProcessingServiceMock.Object,
            folderRoleEventProcessingServiceMock.Object
        );
    }

    private static FolderRole CreateRandomFolderRole() => Builder<FolderRole>.CreateNew().Build();
}









