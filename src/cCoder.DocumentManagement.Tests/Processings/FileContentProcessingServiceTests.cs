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
    private User currentUser = ToLocalUser(TestUsers.WithoutPrivileges());
    private readonly Mock<IFileContentService> fileContentServiceMock = new();
    private readonly FileContentProcessingService fileContentProcessingService;

    public FileContentProcessingServiceTests()
    {
        fileContentProcessingService = new FileContentProcessingService(fileContentServiceMock.Object);
    }

    private static FileContent CreateRandomFileContent() =>
        Builder<FileContent>
            .CreateNew()
            .With(x => x.Id = Guid.NewGuid())
            .With(x => x.FileId = Guid.NewGuid())
            .With(x => x.Description = $"Description-{Guid.NewGuid():N}")
            .With(x => x.Size = "1KB")
            .With(x => x.CreatedBy = "test-user")
            .With(x => x.CreatedOn = DateTimeOffset.UtcNow)
            .With(x => x.Version = 1)
            .With(x => x.RawData = new byte[] { 1, 2, 3 })
            .With(x => x.File = null)
            .Build();

    private static User ToLocalUser(cCoder.Data.Models.Security.User user) =>
        user == null ? null : new User { Id = user.Id, Roles = [] };
}












