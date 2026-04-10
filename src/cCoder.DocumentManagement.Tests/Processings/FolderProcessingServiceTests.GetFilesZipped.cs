using System.IO.Compression;
using System.Security;
using cCoder.Data;
using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;
using DMSResult = cCoder.DocumentManagement.Models.DMSResult;
using DmsFile = cCoder.Data.Models.DMS.File;
using DmsPath = cCoder.DocumentManagement.Models.Path;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FolderProcessingServiceTests
{
    [Fact]
    public void ShouldThrowSecurityExceptionWhenFileDoesNotExistForGetFilesZipped()
    {
        // Given
        App app = CreateRandomAppForTests();
        DmsPath filePath = new("docs/file.txt");
        fileServiceMock
            .Setup(x => x.GetByPathWithFolderAndContents(app.Id, filePath.Lowered, false))
            .Returns((DmsFile)null);

        // When
        Action act = () => folderProcessingService.GetFilesZipped(app, [filePath]);

        // Then
        act.Should().Throw<SecurityException>().WithMessage("Access Denied!");
        fileServiceMock.Verify(
            x => x.GetByPathWithFolderAndContents(app.Id, filePath.Lowered, false),
            Times.Once
        );
        fileServiceMock.VerifyNoOtherCalls();
        folderServiceMock.VerifyNoOtherCalls();
        fileContentServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldReturnZippedFilesAndFoldersWhenGetFilesZipped()
    {
        // Given
        App app = CreateRandomAppForTests();
        DmsPath directFilePath = new("docs/direct.txt");
        DmsPath folderPath = new("docs");

        DmsFile directFile = new()
        {
            Id = Guid.NewGuid(),
            FolderId = Guid.NewGuid(),
            Name = "direct.txt",
            Path = directFilePath.Lowered,
            Contents =
            [
                new FileContent
                {
                    Id = Guid.NewGuid(),
                    FileId = Guid.NewGuid(),
                    Version = 1,
                    RawData = [7, 8, 9],
                },
            ],
        };
        directFile.Contents.First().FileId = directFile.Id;

        Folder rootFolder = CreateRandomFolder();
        rootFolder.AppId = app.Id;
        rootFolder.Name = "docs";
        rootFolder.Path = "docs";

        DmsFile nestedFile = new()
        {
            Id = Guid.NewGuid(),
            FolderId = rootFolder.Id,
            Name = "nested.txt",
            Path = "docs/nested.txt",
        };

        FileContent nestedContent = new()
        {
            Id = Guid.NewGuid(),
            FileId = nestedFile.Id,
            Version = 1,
            RawData = [1, 2, 3],
        };

        fileServiceMock
            .Setup(x => x.GetByPathWithFolderAndContents(app.Id, directFilePath.Lowered, false))
            .Returns(directFile);
        folderServiceMock.Setup(x => x.GetByPath(app.Id, folderPath.Lowered, false)).Returns(rootFolder);
        folderServiceMock.Setup(x => x.GetAll(false)).Returns(new[] { rootFolder }.AsQueryable());
        fileServiceMock.Setup(x => x.GetAll(false)).Returns(new[] { nestedFile }.AsQueryable());
        fileContentServiceMock.Setup(x => x.GetAll(false)).Returns(new[] { nestedContent }.AsQueryable());

        // When
        DMSResult result = folderProcessingService.GetFilesZipped(app, [directFilePath, folderPath]);

        // Then
        using ZipArchive zip = new(result.Data, ZipArchiveMode.Read);
        zip.Entries.Select(entry => entry.FullName)
            .Should()
            .BeEquivalentTo(["direct.txt", "docs/", "docs/nested.txt"]);

        fileServiceMock.Verify(
            x => x.GetByPathWithFolderAndContents(app.Id, directFilePath.Lowered, false),
            Times.Once
        );
        folderServiceMock.Verify(x => x.GetByPath(app.Id, folderPath.Lowered, false), Times.Once);
        folderServiceMock.Verify(x => x.GetAll(false), Times.Once);
        fileServiceMock.Verify(x => x.GetAll(false), Times.Once);
        fileContentServiceMock.Verify(x => x.GetAll(false), Times.Once);
        fileServiceMock.VerifyNoOtherCalls();
        folderServiceMock.VerifyNoOtherCalls();
        fileContentServiceMock.VerifyNoOtherCalls();
    }
}

