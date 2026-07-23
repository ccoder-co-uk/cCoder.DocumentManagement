// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        IQueryable<DataFolder> folders = new[] { ToExternalFolder(folder: folder) }.AsQueryable();

        folderBrokerMock.Setup(expression: x => x.GetAllFolders(false)).Returns(value: folders);

        // When
        IQueryable<Folder> result = folderService.GetAll();

        // Then
        result.Should().BeEquivalentTo(expectation: new[] { folder }.AsQueryable());
        folderBrokerMock.Verify(expression: x => x.GetAllFolders(false), times: Times.Once);
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

        folderBrokerMock.Setup(expression: x => x.GetAllFolders(false)).Returns(value: folders);

        // When
        Folder result = folderService.GetAll().Single();

        // Then
        result.SubFolders.Should().ContainSingle();
        result.SubFolders.Single().Id.Should().Be(expected: childId);
        result.Files.Should().ContainSingle();
        result.Files.Single().Id.Should().Be(expected: fileId);
        folderBrokerMock.Verify(expression: x => x.GetAllFolders(false), times: Times.Once);
        folderBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }
}