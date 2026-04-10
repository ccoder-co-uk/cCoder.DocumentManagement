using cCoder.DocumentManagement.Brokers.Storage;
using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Foundations;
using FizzWare.NBuilder;
using Moq;
using DataFolder = cCoder.Data.Models.DMS.Folder;
using IAuthorizationBroker = cCoder.DocumentManagement.Brokers.IAuthorizationBroker;


namespace cCoder.Core.Services.Tests.DMS.Foundations;

public partial class FolderServiceTests
{
    private readonly Mock<IFolderBroker> folderBrokerMock;
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock;
    private readonly FolderService folderService;

    public FolderServiceTests()
    {
        folderBrokerMock = new Mock<IFolderBroker>(MockBehavior.Strict);
        authorizationBrokerMock = new Mock<IAuthorizationBroker>(MockBehavior.Strict);
        folderService = new FolderService(folderBrokerMock.Object, authorizationBrokerMock.Object);
    }

    private static Folder CreateRandomFolder(Guid id = default, int appId = 7)
    {
        Folder folder = Builder<Folder>
            .CreateNew()
            .With(x => x.Id = id == Guid.Empty ? Guid.NewGuid() : id)
            .With(x => x.AppId = appId)
            .With(x => x.Name = $"Folder-{Guid.NewGuid():N}")
            .With(x => x.Path = $"/folder/{Guid.NewGuid():N}")
            .Build();

        return folder;
    }

    private static DataFolder ToExternalFolder(Folder folder) =>
        folder == null
            ? null
            : new DataFolder
            {
                Id = folder.Id,
                AppId = folder.AppId,
                ParentId = folder.ParentId,
                Name = folder.Name,
                Path = folder.Path,
                DeletedOn = folder.DeletedOn,
            };
}














