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
    public void ShouldDelegateToFoundationServiceWhenGet()
    {
        // Given
        Folder entity = CreateRandomFolder();
        var id = entity.Id;
        folderServiceMock.Setup(x => x.Get(id)).Returns(entity);

        // When
        Folder result = folderProcessingService.Get(id);

        // Then
        result.Should().BeSameAs(entity);
        folderServiceMock.Verify(x => x.Get(id), Times.Once);
        folderServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldThrowInvalidOperationExceptionWhenGetForFilePath()
    {
        // Given
        App app = CreateRandomAppForTests();
        DmsPath filePath = new("docs/file.txt");

        // When
        Action act = () => folderProcessingService.Get(app, filePath);

        // Then
        act.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("To get a file, use file processing operations.");
        folderServiceMock.VerifyNoOtherCalls();
        fileServiceMock.VerifyNoOtherCalls();
        fileContentServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldThrowSecurityExceptionWhenFolderDoesNotExistForGet()
    {
        // Given
        App app = CreateRandomAppForTests();
        DmsPath folderPath = new("docs");
        folderServiceMock.Setup(x => x.GetByPath(app.Id, folderPath.Lowered, false)).Returns((Folder)null);

        // When
        Action act = () => folderProcessingService.Get(app, folderPath);

        // Then
        act.Should().Throw<SecurityException>().WithMessage("Access Denied!");
        folderServiceMock.Verify(x => x.GetByPath(app.Id, folderPath.Lowered, false), Times.Once);
        folderServiceMock.VerifyNoOtherCalls();
        fileServiceMock.VerifyNoOtherCalls();
        fileContentServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldReturnZippedFolderContentsWhenGet()
    {
        // Given
        App app = CreateRandomAppForTests();
        Folder rootFolder = CreateRandomFolder();
        rootFolder.AppId = app.Id;
        rootFolder.Name = "docs";
        rootFolder.Path = "docs";

        Folder childFolder = CreateRandomFolder();
        childFolder.AppId = app.Id;
        childFolder.ParentId = rootFolder.Id;
        childFolder.Name = "nested";
        childFolder.Path = "docs/nested";

        DmsFile rootFile = new()
        {
            Id = Guid.NewGuid(),
            FolderId = rootFolder.Id,
            Name = "root.txt",
            Path = "docs/root.txt",
        };

        DmsFile childFile = new()
        {
            Id = Guid.NewGuid(),
            FolderId = childFolder.Id,
            Name = "child.txt",
            Path = "docs/nested/child.txt",
        };

        FileContent rootContent = new()
        {
            Id = Guid.NewGuid(),
            FileId = rootFile.Id,
            Version = 1,
            RawData = [1, 2, 3],
        };

        FileContent childContent = new()
        {
            Id = Guid.NewGuid(),
            FileId = childFile.Id,
            Version = 1,
            RawData = [4, 5, 6],
        };

        folderServiceMock
            .Setup(x => x.GetByPath(app.Id, rootFolder.Path, false))
            .Returns(rootFolder);
        folderServiceMock
            .Setup(x => x.GetAll(false))
            .Returns(new[] { rootFolder, childFolder }.AsQueryable());
        fileServiceMock
            .Setup(x => x.GetAll(false))
            .Returns(new[] { rootFile, childFile }.AsQueryable());
        fileContentServiceMock
            .Setup(x => x.GetAll(false))
            .Returns(new[] { rootContent, childContent }.AsQueryable());

        // When
        DMSResult result = folderProcessingService.Get(app, new DmsPath(rootFolder.Path));

        // Then
        using ZipArchive zip = new(result.Data, ZipArchiveMode.Read);
        zip.Entries.Select(entry => entry.FullName)
            .Should()
            .BeEquivalentTo(["docs/", "docs/root.txt", "docs/nested/", "docs/nested/child.txt"]);

        folderServiceMock.Verify(x => x.GetByPath(app.Id, rootFolder.Path, false), Times.Once);
        folderServiceMock.Verify(x => x.GetAll(false), Times.Once);
        fileServiceMock.Verify(x => x.GetAll(false), Times.Once);
        fileContentServiceMock.Verify(x => x.GetAll(false), Times.Once);
        folderServiceMock.VerifyNoOtherCalls();
        fileServiceMock.VerifyNoOtherCalls();
        fileContentServiceMock.VerifyNoOtherCalls();
    }

}








