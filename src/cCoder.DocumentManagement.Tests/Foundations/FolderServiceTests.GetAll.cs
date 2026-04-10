using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;
using DataFolder = cCoder.Data.Models.DMS.Folder;


namespace cCoder.Core.Services.Tests.DMS.Foundations;

public partial class FolderServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGetAll()
    {
        // Given
        Folder folder = CreateRandomFolder();
        IQueryable<DataFolder> folders = new[] { ToExternalFolder(folder) }.AsQueryable();

        folderBrokerMock.Setup(x => x.GetAllFolders(false)).Returns(folders);

        // When
        IQueryable<Folder> result = folderService.GetAll();

        // Then
        result.Should().BeEquivalentTo(new[] { folder }.AsQueryable());
        folderBrokerMock.Verify(x => x.GetAllFolders(false), Times.Once);
        folderBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldMapFilesAndSubFoldersWhenGetAll()
    {
        // Given
        Guid folderId = Guid.NewGuid();
        Guid childId = Guid.NewGuid();
        Guid fileId = Guid.NewGuid();
        IQueryable<DataFolder> folders =
            new[]
            {
                new DataFolder
                {
                    Id = folderId,
                    AppId = 7,
                    Name = "Docs",
                    Path = "docs",
                    SubFolders =
                    [
                        new DataFolder
                        {
                            Id = childId,
                            AppId = 7,
                            ParentId = folderId,
                            Name = "Child",
                            Path = "docs/child"
                        }
                    ],
                    Files =
                    [
                        new cCoder.Data.Models.DMS.File
                        {
                            Id = fileId,
                            FolderId = folderId,
                            Name = "file.txt",
                            Path = "docs/file.txt"
                        }
                    ],
                }
            }.AsQueryable();

        folderBrokerMock.Setup(x => x.GetAllFolders(false)).Returns(folders);

        // When
        Folder result = folderService.GetAll().Single();

        // Then
        result.SubFolders.Should().ContainSingle();
        result.SubFolders.Single().Id.Should().Be(childId);
        result.Files.Should().ContainSingle();
        result.Files.Single().Id.Should().Be(fileId);
        folderBrokerMock.Verify(x => x.GetAllFolders(false), Times.Once);
        folderBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }
}








