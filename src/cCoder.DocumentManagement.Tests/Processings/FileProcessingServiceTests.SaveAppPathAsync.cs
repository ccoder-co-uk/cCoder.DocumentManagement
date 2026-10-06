// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FileProcessingServiceTests
{
    [Fact]
    public async Task ShouldRaiseFileAddEventAfterSavingNewFileAppPathAsync()
    {
        // Given
        User actor = ToLocalUser(user: TestUsers.WithPrivilege(privilege: "file_create", appId: 1));
        currentUser = actor;

        Folder folder = CreateFolder(appId: 1, actor: actor);
        string path = "data/masterdata/received/file.csv";
        cCoder.Data.Models.DMS.File createdFile = CreateRandomFile(appId: 1, path: path);
        createdFile.Folder = folder;
        createdFile.FolderId = folder.Id;

        authorizationBrokerMock
            .Setup(expression: x => x.GetCurrentUser())
            .Returns(valueFunction: () => currentUser);

        fileServiceMock
            .Setup(expression: x => x.GetByPath(
                appId: 1,
                path: path,
                ignoreFilters: false))
            .Returns(value: (cCoder.Data.Models.DMS.File)null);

        folderServiceMock
            .Setup(expression: x => x.GetByPathWithRoles(
                appId: 1,
                path: "data/masterdata/received",
                ignoreFilters: false))
            .Returns(value: folder);

        fileServiceMock
            .Setup(expression: x => x.AddFileAsync(newFile: It.IsAny<cCoder.Data.Models.DMS.File>()))
            .ReturnsAsync(value: createdFile);

        fileContentProcessingServiceMock
            .Setup(expression: x => x.AddFileContentAsync(newFileContent: It.IsAny<FileContent>()))
            .ReturnsAsync(value: new FileContent { FileId = createdFile.Id, Version = 1 });

        fileEventServiceMock
            .Setup(expression: x => x.RaiseFileAddEventAsync(entity: createdFile))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await fileProcessingService.SaveAppPathAsync(
            appId: 1,
            path: path,
            content: new MemoryStream(buffer: [1, 2, 3]));

        // Then
        fileEventServiceMock.Verify(
            expression: x => x.RaiseFileAddEventAsync(entity: createdFile),
            times: Times.Once);

        fileServiceMock.Verify(
            expression: x => x.GetWithFolderAndContents(
                fileId: createdFile.Id,
                ignoreFilters: true),
            times: Times.Never);
    }

    [Fact]
    public async Task ShouldRaiseFileUpdateEventAfterSavingExistingFileAppPathAsync()
    {
        // Given
        User actor = ToLocalUser(user: TestUsers.WithPrivilege(privilege: "file_update", appId: 1));
        currentUser = actor;

        Folder folder = CreateFolder(appId: 1, actor: actor);
        string path = "data/masterdata/received/file.csv";
        cCoder.Data.Models.DMS.File existingFile = CreateRandomFile(appId: 1, path: path);
        existingFile.Folder = folder;
        existingFile.FolderId = folder.Id;

        authorizationBrokerMock
            .Setup(expression: x => x.GetCurrentUser())
            .Returns(valueFunction: () => currentUser);

        fileServiceMock
            .Setup(expression: x => x.GetByPath(
                appId: 1,
                path: path,
                ignoreFilters: false))
            .Returns(value: existingFile);

        folderServiceMock
            .Setup(expression: x => x.GetByPathWithRoles(
                appId: 1,
                path: "data/masterdata/received",
                ignoreFilters: false))
            .Returns(value: folder);

        fileContentProcessingServiceMock
            .Setup(expression: x => x.GetAll(ignoreFilters: false))
            .Returns(value: new[]
            {
                new FileContent { FileId = existingFile.Id, Version = 1 }
            }.AsQueryable());

        fileContentProcessingServiceMock
            .Setup(expression: x => x.AddFileContentAsync(newFileContent: It.IsAny<FileContent>()))
            .ReturnsAsync(value: new FileContent { FileId = existingFile.Id, Version = 2 });

        fileEventServiceMock
            .Setup(expression: x => x.RaiseFileUpdateEventAsync(entity: existingFile))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await fileProcessingService.SaveAppPathAsync(
            appId: 1,
            path: path,
            content: new MemoryStream(buffer: [4, 5, 6]));

        // Then
        fileEventServiceMock.Verify(
            expression: x => x.RaiseFileUpdateEventAsync(entity: existingFile),
            times: Times.Once);

        fileServiceMock.Verify(
            expression: x => x.GetWithFolderAndContents(
                fileId: existingFile.Id,
                ignoreFilters: true),
            times: Times.Never);
    }

    private static Folder CreateFolder(int appId, User actor) =>
        new()
        {
            Id = Guid.NewGuid(),
            AppId = appId,
            Name = "received",
            Path = "data/masterdata/received",
            Roles =
            [
                new FolderRole
                {
                    RoleId = actor.Roles.First().RoleId,
                    Role = actor.Roles.First().Role,
                },
            ],
        };
}