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
        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(() => currentUser);
        User actor = ToLocalUser(TestUsers.WithPrivilege("file_create", 1));
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
        cCoder.Data.Models.DMS.File createdFile = CreateRandomFile(file.Id, 1, "root/file.txt");
        createdFile.FolderId = folderId;
        createdFile.Folder = folder;
        createdFile.Contents = [];
        currentUser = actor;

        folderServiceMock.Setup(x => x.GetWithRoles(folderId, true)).Returns(folder);
        fileServiceMock
            .Setup(x => x.AddAsync(It.IsAny<cCoder.Data.Models.DMS.File>()))
            .ReturnsAsync(createdFile);
        fileServiceMock
            .Setup(x => x.GetWithFolderAndContents(createdFile.Id, true))
            .Returns(createdFile);

        // When
        cCoder.Data.Models.DMS.File result = await fileProcessingService.AddAsync(file);

        // Then
        result.Should().BeEquivalentTo(createdFile);
        fileServiceMock.Verify(x => x.AddAsync(It.IsAny<cCoder.Data.Models.DMS.File>()), Times.Once);
        fileServiceMock.Verify(x => x.GetWithFolderAndContents(createdFile.Id, true), Times.Once);
        folderServiceMock.Verify(x => x.GetWithRoles(folderId, true), Times.Once);
        fileServiceMock.VerifyNoOtherCalls();
        folderServiceMock.VerifyNoOtherCalls();
    }

}










