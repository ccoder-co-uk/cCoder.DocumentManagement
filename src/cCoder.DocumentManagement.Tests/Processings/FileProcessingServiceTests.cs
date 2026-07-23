// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Foundations;
using cCoder.DocumentManagement.Services.Processings;
using Moq;
using IAuthorizationBroker = cCoder.DocumentManagement.Brokers.IAuthorizationBroker;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FileProcessingServiceTests
{
    private readonly Mock<IFileService> fileServiceMock = new();
    private readonly Mock<IFolderService> folderServiceMock = new();
    private readonly Mock<IFileContentService> fileContentServiceMock = new();
    private User currentUser = ToLocalUser(user: TestUsers.WithoutPrivileges());
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock = new();
    private readonly Mock<IFileContentProcessingService> fileContentProcessingServiceMock = new();
    private readonly FileProcessingService fileProcessingService;

    public FileProcessingServiceTests()
    {
        fileProcessingService = new FileProcessingService(
            service: fileServiceMock.Object,
            folderService: folderServiceMock.Object,
            fileContentService: fileContentServiceMock.Object,
            fileContentProcessingService: fileContentProcessingServiceMock.Object,
            authorizationBroker: authorizationBrokerMock.Object
        );
    }

    private static cCoder.Data.Models.DMS.File CreateRandomFile(
        Guid id = default,
        int appId = 1,
        string path = "root/file.txt"
    ) =>
        new()
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id,
            FolderId = Guid.NewGuid(),
            Name = "file.txt",
            Path = path,
            Description = "Description",
            MimeType = "text/plain",
            Size = "1024",
            CreatedBy = "test-user",
            CreatedOn = DateTimeOffset.UtcNow,
            Folder = new Folder
            {
                Id = Guid.NewGuid(),
                AppId = appId,
                Name = "root",
                Path = "root",
                Roles = [],
            },
        };

    private static User ToLocalUser(cCoder.Data.Models.Security.User user) =>
        user == null
            ? null
            : new User
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                Email = user.Email,
                IsActive = user.IsActive,
                Roles = user.Roles?
                    .Select(selector: role => new UserRole
                    {
                        UserId = role.UserId,
                        RoleId = role.RoleId,
                        Role = role.Role == null
                            ? null
                            : new Role
                            {
                                Id = role.Role.Id,
                                AppId = role.Role.AppId,
                                Name = role.Role.Name,
                                Description = role.Role.Description,
                                Privs = role.Role.Privs,
                            },
                    })
                    .ToArray(),
            };
}