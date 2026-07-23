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
using Microsoft.Extensions.Logging;
using Moq;
using DataApp = cCoder.Data.Models.CMS.App;
using DataFile = cCoder.Data.Models.DMS.File;
using DataFileContent = cCoder.Data.Models.DMS.FileContent;
using DataFolder = cCoder.Data.Models.DMS.Folder;
using DataFolderRole = cCoder.Data.Models.Security.FolderRole;
using DataRole = cCoder.Data.Models.Security.Role;
using IAuthorizationBroker = cCoder.DocumentManagement.Brokers.IAuthorizationBroker;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FolderProcessingServiceTests
{
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock = new();
    private readonly Mock<IFolderService> folderServiceMock = new();
    private readonly Mock<IFolderRoleService> folderRoleServiceMock = new();
    private readonly Mock<IRoleService> roleServiceMock = new();
    private readonly Mock<IFileService> fileServiceMock = new();
    private readonly Mock<IFileContentService> fileContentServiceMock = new();
    private readonly Mock<IFileProcessingService> fileProcessingServiceMock = new();
    private readonly Mock<ILogger<FolderProcessingService>> loggerMock = new();
    private User currentUser = ToLocalUser(user: TestUsers.WithoutPrivileges());
    private readonly FolderProcessingService folderProcessingService;

    public FolderProcessingServiceTests()
    {
        folderProcessingService = new FolderProcessingService(
            service: folderServiceMock.Object,
            folderRoleService: folderRoleServiceMock.Object,
            roleService: roleServiceMock.Object,
            fileService: fileServiceMock.Object,
            fileContentService: fileContentServiceMock.Object,
            fileProcessingService: fileProcessingServiceMock.Object,
            authorizationBroker: authorizationBrokerMock.Object
        );
    }

    private static Folder CreateRandomFolder() =>
        new()
        {
            Id = Guid.NewGuid(),
            AppId = 1,
            Name = $"Folder-{Guid.NewGuid():N}",
            Path = $"folder-{Guid.NewGuid():N}",
            App = CreateRandomAppForTests(),
            Roles = [],
            Files = [],
            SubFolders = [],
        };

    private static App CreateRandomAppForTests() =>
        Builder<App>
            .CreateNew()
            .With(func: x => x.Id = 1)
            .With(func: x => x.Name = $"App-{Guid.NewGuid():N}")
            .Build();

    private static User ToLocalUser(cCoder.Data.Models.Security.User user) =>
        user == null
            ? null
            : new User
            {
                Id = user.Id,
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

    private static DataFolder ToDataFolder(Folder folder) =>
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
                App = folder.App == null ? new DataApp { Id = folder.AppId, Name = "App" } : new DataApp
                {
                    Id = folder.App.Id,
                    Name = folder.App.Name,
                },
                Roles = folder.Roles?.Select(selector: role => new DataFolderRole
                {
                    FolderId = role.FolderId,
                    RoleId = role.RoleId,
                    Role = role.Role == null
                        ? null
                        : new DataRole
                        {
                            Id = role.Role.Id,
                            AppId = role.Role.AppId,
                            Name = role.Role.Name,
                            Description = role.Role.Description,
                            Privs = role.Role.Privs,
                        },
                })
                  .ToList() ?? [],
                Files = folder.Files?.Select(selector: file => new DataFile
                {
                    Id = file.Id,
                    FolderId = file.FolderId,
                    Name = file.Name,
                    Description = file.Description,
                    Path = file.Path,
                    MimeType = file.MimeType,
                    Size = file.Size,
                    CreatedBy = file.CreatedBy,
                    CreatedOn = file.CreatedOn,
                    DeletedOn = file.DeletedOn,
                    Contents = file.Contents?.Select(selector: content => new DataFileContent
                    {
                        Id = content.Id,
                        FileId = content.FileId,
                        Description = content.Description,
                        Size = content.Size,
                        CreatedBy = content.CreatedBy,
                        CreatedOn = content.CreatedOn,
                        Version = content.Version,
                        RawData = content.RawData,
                    })
                      .ToList() ?? [],
                })
                  .ToList() ?? [],
            };
}