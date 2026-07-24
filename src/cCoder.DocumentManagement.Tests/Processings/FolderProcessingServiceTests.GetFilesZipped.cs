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
    public void ShouldThrowSecurityExceptionWhenFileDoesNotExistForGetFilesZipped()
    {
        // Given
        App app = CreateRandomAppForTests();
        DmsPath filePath = new(path: "docs/file.txt");

        fileServiceMock
            .Setup(expression: x => x.GetByPathWithFolderAndContents(appId: app.Id, path: filePath.Lowered, ignoreFilters: false))
            .Returns(value: (DmsFile)null);

        // When
        Action act = () => folderProcessingService.GetFilesZippedAppPath(appId: app.Id, paths: [filePath]);

        // Then
        act.Should()
            .Throw<DocumentManagementServiceException>()
            .WithInnerException(innerException: typeof(SecurityException));

        fileServiceMock.Verify(
            expression: x => x.GetByPathWithFolderAndContents(appId: app.Id, path: filePath.Lowered, ignoreFilters: false),
            times: Times.Once
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
        DmsPath directFilePath = new(path: "docs/direct.txt");
        DmsPath folderPath = new(path: "docs");

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
            .Setup(expression: x => x.GetByPathWithFolderAndContents(appId: app.Id, path: directFilePath.Lowered, ignoreFilters: false))
            .Returns(value: directFile);

        folderServiceMock.Setup(expression: x => x.GetByPath(appId: app.Id, path: folderPath.Lowered, ignoreFilters: false))
            .Returns(value: rootFolder);

        folderServiceMock.Setup(expression: x => x.GetAll(ignoreFilters: false))
            .Returns(value: new[] { rootFolder }.AsQueryable());

        fileServiceMock.Setup(expression: x => x.GetAll(ignoreFilters: false))
            .Returns(value: new[] { nestedFile }.AsQueryable());

        fileContentServiceMock.Setup(expression: x => x.GetAll(ignoreFilters: false))
            .Returns(value: new[] { nestedContent }.AsQueryable());

        // When
        DMSResult result = folderProcessingService.GetFilesZippedAppPath(appId: app.Id, paths: [directFilePath, folderPath]);

        // Then
        using ZipArchive zip = new(stream: result.Data, mode: ZipArchiveMode.Read);

        zip.Entries.Select(selector: entry => entry.FullName)
            .Should()
            .BeEquivalentTo(expectation: ["direct.txt", "docs/", "docs/nested.txt"]);

        fileServiceMock.Verify(
            expression: x => x.GetByPathWithFolderAndContents(appId: app.Id, path: directFilePath.Lowered, ignoreFilters: false),
            times: Times.Once
        );

        folderServiceMock.Verify(expression: x => x.GetByPath(appId: app.Id, path: folderPath.Lowered, ignoreFilters: false), times: Times.Once);
        folderServiceMock.Verify(expression: x => x.GetAll(ignoreFilters: false), times: Times.Once);
        fileServiceMock.Verify(expression: x => x.GetAll(ignoreFilters: false), times: Times.Once);
        fileContentServiceMock.Verify(expression: x => x.GetAll(ignoreFilters: false), times: Times.Once);
        fileServiceMock.VerifyNoOtherCalls();
        folderServiceMock.VerifyNoOtherCalls();
        fileContentServiceMock.VerifyNoOtherCalls();
    }
}