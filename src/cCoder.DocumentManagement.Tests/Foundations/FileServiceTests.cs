// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        fileBrokerMock = new Mock<IFileBroker>(behavior: MockBehavior.Strict);
        authorizationBrokerMock = new Mock<IAuthorizationBroker>(behavior: MockBehavior.Strict);
        fileService = new FileService(fileBroker: fileBrokerMock.Object, authorizationBroker: authorizationBrokerMock.Object);
    }

    private static FileEntity CreateRandomFile(Guid id = default, Guid folderId = default)
    {
        FileEntity file = Builder<FileEntity>
            .CreateNew()
            .With(func: x => x.Id = id == Guid.Empty ? Guid.NewGuid() : id)
            .With(func: x => x.FolderId = folderId == Guid.Empty ? Guid.NewGuid() : folderId)
            .With(func: x => x.Name = $"File-{Guid.NewGuid():N}")
            .With(func: x => x.Description = $"Description-{Guid.NewGuid():N}")
            .With(func: x => x.Path = $"/files/{Guid.NewGuid():N}")
            .With(func: x => x.MimeType = "application/octet-stream")
            .With(func: x => x.CreatedBy = "tester")
            .With(func: x => x.Size = "1024")
            .With(func: x => x.CreatedOn = DateTimeOffset.UtcNow)
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