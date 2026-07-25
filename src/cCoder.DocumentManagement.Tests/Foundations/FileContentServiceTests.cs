// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Brokers.Storage;
using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Foundations;
using FizzWare.NBuilder;
using Moq;
using DataFileContent = cCoder.Data.Models.DMS.FileContent;
using IAuthorizationBroker = cCoder.DocumentManagement.Brokers.IAuthorizationBroker;


namespace cCoder.Core.Services.Tests.DMS.Foundations;

public partial class FileContentServiceTests
{
    private readonly Mock<IFileContentBroker> fileContentBrokerMock;
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock;
    private readonly FileContentService fileContentService;

    public FileContentServiceTests()
    {
        fileContentBrokerMock = new Mock<IFileContentBroker>(behavior: MockBehavior.Strict);
        authorizationBrokerMock = new Mock<IAuthorizationBroker>(behavior: MockBehavior.Strict);
        fileContentService = new FileContentService(
            fileContentBroker: fileContentBrokerMock.Object,
            authorizationBroker: authorizationBrokerMock.Object
        );
    }

    private static FileContent CreateRandomFileContent(Guid id = default, Guid fileId = default)
    {
        FileContent fileContent = Builder<FileContent>
            .CreateNew()
            .With(func: x => x.Id = id == Guid.Empty ? Guid.NewGuid() : id)
            .With(func: x => x.FileId = fileId == Guid.Empty ? Guid.NewGuid() : fileId)
            .With(func: x => x.Description = $"Description-{Guid.NewGuid():N}")
            .With(func: x => x.Size = "1024")
            .With(func: x => x.CreatedBy = "tester")
            .With(func: x => x.CreatedOn = DateTimeOffset.UtcNow)
            .With(func: x => x.Version = 1)
            .With(func: x => x.RawData = [1, 2, 3])
            .Build();

        return fileContent;
    }

    private static DataFileContent ToExternalFileContent(FileContent fileContent) =>
        fileContent == null
            ? null
            : new DataFileContent
            {
                Id = fileContent.Id,
                FileId = fileContent.FileId,
                Description = fileContent.Description,
                Size = fileContent.Size,
                CreatedBy = fileContent.CreatedBy,
                CreatedOn = fileContent.CreatedOn,
                Version = fileContent.Version,
                RawData = fileContent.RawData,
            };
}