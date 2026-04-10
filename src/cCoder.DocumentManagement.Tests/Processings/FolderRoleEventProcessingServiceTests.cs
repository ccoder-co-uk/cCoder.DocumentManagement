using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Foundations.Events;
using cCoder.DocumentManagement.Services.Processings;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FolderRoleEventProcessingServiceTests
{
    private readonly Mock<IFolderRoleEventService> folderRoleEventServiceMock;
    private readonly FolderRoleEventProcessingService service;

    public FolderRoleEventProcessingServiceTests()
    {
        folderRoleEventServiceMock = new Mock<IFolderRoleEventService>(MockBehavior.Strict);
        service = new FolderRoleEventProcessingService(folderRoleEventServiceMock.Object);
    }

    private static FolderRole CreateRandomFolderRole() =>
        Builder<FolderRole>.CreateNew().Build();
}











