using FluentAssertions;
using Moq;
using Xunit;
using DataApp = cCoder.Data.Models.CMS.App;
using DataFile = cCoder.Data.Models.DMS.File;
using DataFolder = cCoder.Data.Models.DMS.Folder;
using DataFolderRole = cCoder.Data.Models.Security.FolderRole;
using DataRole = cCoder.Data.Models.Security.Role;
using LocalFolder = cCoder.Data.Models.DMS.Folder;


namespace cCoder.Core.Services.Tests.DMS.Foundations;

public partial class FolderServiceTests
{
    [Fact]
    public void ShouldMapGraphWhenGetForUpdate()
    {
        // Given
        Guid folderId = Guid.NewGuid();
        Guid parentId = Guid.NewGuid();
        Guid childId = Guid.NewGuid();
        Guid fileId = Guid.NewGuid();
        Guid roleId = Guid.NewGuid();

        var dataFolder = new DataFolder
        {
            Id = folderId,
            AppId = 7,
            Name = "Docs",
            Path = "docs",
            App = new DataApp { Id = 7, Name = "App" },
            Parent = new DataFolder { Id = parentId, AppId = 7, Name = "Parent", Path = "parent" },
            SubFolders =
            [
                new DataFolder { Id = childId, AppId = 7, ParentId = folderId, Name = "Child", Path = "docs/child" }
            ],
            Files =
            [
                new DataFile { Id = fileId, FolderId = folderId, Name = "file.txt", Path = "docs/file.txt" }
            ],
            Roles =
            [
                new DataFolderRole
                {
                    FolderId = folderId,
                    RoleId = roleId,
                    Role = new DataRole { Id = roleId, AppId = 7, Name = "Admin", Privs = "folder_update" }
                }
            ],
        };

        folderBrokerMock.Setup(x => x.GetFolderForUpdate(folderId, true)).Returns(dataFolder);

        // When
        LocalFolder result = folderService.GetForUpdate(folderId, true);

        // Then
        result.Should().NotBeNull();
        result.App.Should().NotBeNull();
        result.Parent.Should().NotBeNull();
        result.SubFolders.Should().ContainSingle();
        result.Files.Should().ContainSingle();
        result.Roles.Should().ContainSingle();
        result.Files.Single().Id.Should().Be(fileId);
        result.SubFolders.Single().Id.Should().Be(childId);
        result.Roles.Single().RoleId.Should().Be(roleId);
        folderBrokerMock.Verify(x => x.GetFolderForUpdate(folderId, true), Times.Once);
        folderBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldMapParentAndRolesWhenGetByPathWithParentAndRoles()
    {
        // Given
        var dataFolder = new DataFolder
        {
            Id = Guid.NewGuid(),
            AppId = 7,
            Name = "Docs",
            Path = "docs",
            Parent = new DataFolder { Id = Guid.NewGuid(), AppId = 7, Name = "Root", Path = "root" },
            Roles =
            [
                new DataFolderRole
                {
                    FolderId = Guid.NewGuid(),
                    RoleId = Guid.NewGuid(),
                    Role = new DataRole { Id = Guid.NewGuid(), AppId = 7, Name = "Admin", Privs = "folder_update" }
                }
            ],
        };

        folderBrokerMock
            .Setup(x => x.GetFolderByPathWithParentAndRoles(7, "docs", true))
            .Returns(dataFolder);

        // When
        LocalFolder result = folderService.GetByPathWithParentAndRoles(7, "docs", true);

        // Then
        result.Should().NotBeNull();
        result.Parent.Should().NotBeNull();
        result.Roles.Should().ContainSingle();
        folderBrokerMock.Verify(x => x.GetFolderByPathWithParentAndRoles(7, "docs", true), Times.Once);
        folderBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldMapSubFoldersAndFilesWhenGetByPathWithSubFoldersAndFiles()
    {
        // Given
        var dataFolder = new DataFolder
        {
            Id = Guid.NewGuid(),
            AppId = 7,
            Name = "Docs",
            Path = "docs",
            SubFolders =
            [
                new DataFolder { Id = Guid.NewGuid(), AppId = 7, Name = "Child", Path = "docs/child" }
            ],
            Files =
            [
                new DataFile { Id = Guid.NewGuid(), FolderId = Guid.NewGuid(), Name = "file.txt", Path = "docs/file.txt" }
            ],
        };

        folderBrokerMock
            .Setup(x => x.GetFolderByPathWithSubFoldersAndFiles(7, "docs", false))
            .Returns(dataFolder);

        // When
        LocalFolder result = folderService.GetByPathWithSubFoldersAndFiles(7, "docs", false);

        // Then
        result.Should().NotBeNull();
        result.SubFolders.Should().ContainSingle();
        result.Files.Should().ContainSingle();
        folderBrokerMock.Verify(x => x.GetFolderByPathWithSubFoldersAndFiles(7, "docs", false), Times.Once);
        folderBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }
}


