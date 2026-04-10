using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services;
using cCoder.DocumentManagement.Services.Orchestrations;
using cCoder.DocumentManagement.Services.Processings;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.DMS.Orchestrations;

public partial class DmsOrchestrationServiceTests
{
    private readonly Mock<IDocumentManagementCurrentAppResolver> currentAppResolverMock;
    private readonly Mock<IFileProcessingService> fileProcessingServiceMock;
    private readonly Mock<IFolderProcessingService> folderProcessingServiceMock;
    private readonly DmsOrchestrationService orchestrationService;

    public DmsOrchestrationServiceTests()
    {
        currentAppResolverMock = new Mock<IDocumentManagementCurrentAppResolver>(MockBehavior.Strict);
        fileProcessingServiceMock = new Mock<IFileProcessingService>(MockBehavior.Strict);
        folderProcessingServiceMock = new Mock<IFolderProcessingService>(MockBehavior.Strict);
        orchestrationService = new DmsOrchestrationService(
            currentAppResolverMock.Object,
            fileProcessingServiceMock.Object,
            folderProcessingServiceMock.Object
        );
    }

    private static App CreateRandomApp() =>
        Builder<App>.CreateNew()
            .With(app => app.Domain = $"app-{Guid.NewGuid():N}.test")
            .With(app => app.Roles = [])
            .With(app => app.Folders = [])
            .Build();
}







