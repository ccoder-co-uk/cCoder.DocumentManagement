using cCoder.DocumentManagement.Brokers.Storage;
using cCoder.DocumentManagement.Services.Foundations;
using FizzWare.NBuilder;
using Moq;
using DataFile = cCoder.Data.Models.DMS.File;
using FileEntity = cCoder.Data.Models.DMS.File;
using IAuthorizationBroker = cCoder.DocumentManagement.Brokers.IAuthorizationBroker;


namespace cCoder.Core.Services.Tests.DMS.Foundations;

public partial class FileServiceTests
{
    private readonly Mock<IFileBroker> fileBrokerMock;
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock;
    private readonly FileService fileService;

    public FileServiceTests()
    {
        fileBrokerMock = new Mock<IFileBroker>(MockBehavior.Strict);
        authorizationBrokerMock = new Mock<IAuthorizationBroker>(MockBehavior.Strict);
        fileService = new FileService(fileBrokerMock.Object, authorizationBrokerMock.Object);
    }

    private static FileEntity CreateRandomFile(Guid id = default, Guid folderId = default)
    {
        FileEntity file = Builder<FileEntity>
            .CreateNew()
            .With(x => x.Id = id == Guid.Empty ? Guid.NewGuid() : id)
            .With(x => x.FolderId = folderId == Guid.Empty ? Guid.NewGuid() : folderId)
            .With(x => x.Name = $"File-{Guid.NewGuid():N}")
            .With(x => x.Description = $"Description-{Guid.NewGuid():N}")
            .With(x => x.Path = $"/files/{Guid.NewGuid():N}")
            .With(x => x.MimeType = "application/octet-stream")
            .With(x => x.CreatedBy = "tester")
            .With(x => x.Size = "1024")
            .With(x => x.CreatedOn = DateTimeOffset.UtcNow)
            .Build();

        return file;
    }

    private static DataFile ToExternalFile(FileEntity file) =>
        file == null
            ? null
            : new DataFile
            {
                Id = file.Id,
                FolderId = file.FolderId,
                Name = file.Name,
                Description = file.Description,
                Path = file.Path,
                MimeType = file.MimeType,
                CreatedBy = file.CreatedBy,
                CreatedOn = file.CreatedOn,
                Size = file.Size,
            };
}















