// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Foundations;
using cCoder.DocumentManagement.Services.Processings;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FileContentProcessingServiceTests
{
    private User currentUser = ToLocalUser(user: TestUsers.WithoutPrivileges());
    private readonly Mock<IFileContentService> fileContentServiceMock = new();
    private readonly FileContentProcessingService fileContentProcessingService;

    public FileContentProcessingServiceTests()
    {
        fileContentProcessingService = new FileContentProcessingService(service: fileContentServiceMock.Object);
    }

    private static FileContent CreateRandomFileContent() =>
        Builder<FileContent>
            .CreateNew()
            .With(func: x => x.Id = Guid.NewGuid())
            .With(func: x => x.FileId = Guid.NewGuid())
            .With(func: x => x.Description = $"Description-{Guid.NewGuid():N}")
            .With(func: x => x.Size = "1KB")
            .With(func: x => x.CreatedBy = "test-user")
            .With(func: x => x.CreatedOn = DateTimeOffset.UtcNow)
            .With(func: x => x.Version = 1)
            .With(func: x => x.RawData = new byte[] { 1, 2, 3 })
            .With(func: x => x.File = null)
            .Build();

    private static User ToLocalUser(cCoder.Data.Models.Security.User user) =>
        user == null ? null : new User { Id = user.Id, Roles = [] };
}