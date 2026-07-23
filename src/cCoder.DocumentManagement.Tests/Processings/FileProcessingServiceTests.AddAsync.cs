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


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FileProcessingServiceTests
{
    [Fact]
    public async Task ShouldCreateAndReturnFileWhenUserCanCreateFileForAddAsync()
    {
        // Given
        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(valueFunction: () => currentUser);

        User actor = ToLocalUser(user: TestUsers.WithPrivilege(privilege: "file_create", appId: 1));
        Guid folderId = Guid.NewGuid();

        Folder folder = new()
        {
            Id = folderId,
            AppId = 1,
            Name = "root",
            Path = "root",
            Roles =
            [
                new FolderRole { RoleId = actor.Roles.First().RoleId, Role = actor.Roles.First().Role },
            ],
        };

        cCoder.Data.Models.DMS.File file = CreateRandomFile(path: "file.txt");
        file.FolderId = folderId;
        file.Name = "file.txt";
        file.Path = "file.txt";
        cCoder.Data.Models.DMS.File createdFile = CreateRandomFile(id: file.Id, appId: 1, path: "root/file.txt");
        createdFile.FolderId = folderId;
        createdFile.Folder = folder;
        createdFile.Contents = [];
        currentUser = actor;

        folderServiceMock.Setup(expression: x => x.GetWithRoles(id: folderId, ignoreFilters: true))
            .Returns(value: folder);

        fileServiceMock
            .Setup(expression: x => x.AddAsync(entity: It.IsAny<cCoder.Data.Models.DMS.File>()))
            .ReturnsAsync(value: createdFile);

        fileServiceMock
            .Setup(expression: x => x.GetWithFolderAndContents(id: createdFile.Id, ignoreFilters: true))
            .Returns(value: createdFile);

        // When
        cCoder.Data.Models.DMS.File result = await fileProcessingService.AddAsync(newFile: file);

        // Then
        result.Should()
            .BeEquivalentTo(expectation: createdFile);

        fileServiceMock.Verify(expression: x => x.AddAsync(entity: It.IsAny<cCoder.Data.Models.DMS.File>()), times: Times.Once);
        fileServiceMock.Verify(expression: x => x.GetWithFolderAndContents(id: createdFile.Id, ignoreFilters: true), times: Times.Once);
        folderServiceMock.Verify(expression: x => x.GetWithRoles(id: folderId, ignoreFilters: true), times: Times.Once);
        fileServiceMock.VerifyNoOtherCalls();
        folderServiceMock.VerifyNoOtherCalls();
    }

}