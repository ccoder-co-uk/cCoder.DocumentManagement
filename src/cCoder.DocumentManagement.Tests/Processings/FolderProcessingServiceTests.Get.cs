// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
using DMSResult = cCoder.DocumentManagement.Dependencies.DMSResult;
using DmsFile = cCoder.Data.Models.DMS.File;
using DmsPath = cCoder.DocumentManagement.Dependencies.Path;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FolderProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGet()
    {
        // Given
        Folder entity = CreateRandomFolder();
        var id = entity.Id;

        folderServiceMock.Setup(expression: x => x.Get(folderId: id))
            .Returns(value: entity);

        // When
        Folder result = folderProcessingService.Get(folderId: id);

        // Then
        result.Should()
            .BeSameAs(expected: entity);

        folderServiceMock.Verify(expression: x => x.Get(folderId: id), times: Times.Once);
        folderServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldThrowInvalidOperationExceptionWhenGetForFilePath()
    {
        // Given
        App app = CreateRandomAppForTests();
        DmsPath filePath = new(path: "docs/file.txt");

        // When
        Action act = () => folderProcessingService.GetAppPath(appId: app.Id, path: filePath);

        // Then
        act.Should()
            .Throw<InvalidOperationException>()
            .WithMessage(expectedWildcardPattern: "To get a file, use file processing operations.");

        folderServiceMock.VerifyNoOtherCalls();
        fileServiceMock.VerifyNoOtherCalls();
        fileContentServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldThrowSecurityExceptionWhenFolderDoesNotExistForGet()
    {
        // Given
        App app = CreateRandomAppForTests();
        DmsPath folderPath = new(path: "docs");

        folderServiceMock.Setup(expression: x => x.GetByPath(appId: app.Id, path: folderPath.Lowered, ignoreFilters: false))
            .Returns(value: (Folder)null);

        // When
        Action act = () => folderProcessingService.GetAppPath(appId: app.Id, path: folderPath);

        // Then
        act.Should()
            .Throw<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        folderServiceMock.Verify(expression: x => x.GetByPath(appId: app.Id, path: folderPath.Lowered, ignoreFilters: false), times: Times.Once);
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
            .Setup(expression: x => x.GetByPath(appId: app.Id, path: rootFolder.Path, ignoreFilters: false))
            .Returns(value: rootFolder);

        folderServiceMock
            .Setup(expression: x => x.GetAll(ignoreFilters: false))
            .Returns(value: new[] { rootFolder, childFolder }.AsQueryable());

        fileServiceMock
            .Setup(expression: x => x.GetAll(ignoreFilters: false))
            .Returns(value: new[] { rootFile, childFile }.AsQueryable());

        fileContentServiceMock
            .Setup(expression: x => x.GetAll(ignoreFilters: false))
            .Returns(value: new[] { rootContent, childContent }.AsQueryable());

        // When
        DMSResult result = folderProcessingService.GetAppPath(appId: app.Id, path: new DmsPath(path: rootFolder.Path));

        // Then
        using ZipArchive zip = new(stream: result.Data, mode: ZipArchiveMode.Read);

        zip.Entries.Select(selector: entry => entry.FullName)
            .Should()
            .BeEquivalentTo(expectation: ["docs/", "docs/root.txt", "docs/nested/", "docs/nested/child.txt"]);

        folderServiceMock.Verify(expression: x => x.GetByPath(appId: app.Id, path: rootFolder.Path, ignoreFilters: false), times: Times.Once);
        folderServiceMock.Verify(expression: x => x.GetAll(ignoreFilters: false), times: Times.Once);
        fileServiceMock.Verify(expression: x => x.GetAll(ignoreFilters: false), times: Times.Once);
        fileContentServiceMock.Verify(expression: x => x.GetAll(ignoreFilters: false), times: Times.Once);
        folderServiceMock.VerifyNoOtherCalls();
        fileServiceMock.VerifyNoOtherCalls();
        fileContentServiceMock.VerifyNoOtherCalls();
    }

}
