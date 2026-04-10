using System.Security;
using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;
using DmsPath = cCoder.DocumentManagement.Models.Path;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FolderProcessingServiceTests
{
    [Fact]
    public async Task ShouldDeleteFolderWhenUserCanDeleteForDropAsync()
    {
        // Given
        currentUser = ToLocalUser(TestUsers.WithPrivilege("folder_delete", 1));
        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(() => currentUser);

        App app = CreateRandomAppForTests();
        Folder folder = CreateRandomFolder();
        folder.AppId = app.Id;
        folder.Path = "docs";
        folder.Roles =
        [
            new FolderRole
            {
                FolderId = folder.Id,
                RoleId = currentUser.Roles.Single().RoleId,
                Role = new Role
                {
                    Id = currentUser.Roles.Single().RoleId,
                    AppId = app.Id,
                    Privileges = ["folder_delete"],
                },
            },
        ];

        folderServiceMock
            .Setup(x => x.GetByPathWithRoles(app.Id, folder.Path, false))
            .Returns(folder);
        folderServiceMock.Setup(x => x.DeleteAsync(folder.Id)).Returns(ValueTask.CompletedTask);

        // When
        await folderProcessingService.DropAsync(app, new DmsPath(folder.Path));

        // Then
        folderServiceMock.Verify(x => x.GetByPathWithRoles(app.Id, folder.Path, false), Times.Once);
        folderServiceMock.Verify(x => x.DeleteAsync(folder.Id), Times.Once);
        folderServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.GetCurrentUser(), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenFolderCannotBeDeletedForDropAsync()
    {
        // Given
        currentUser = ToLocalUser(TestUsers.WithoutPrivileges());
        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(() => currentUser);

        App app = CreateRandomAppForTests();
        DmsPath path = new("docs");
        folderServiceMock
            .Setup(x => x.GetByPathWithRoles(app.Id, path.Lowered, false))
            .Returns((Folder)null);

        // When
        Func<Task> act = async () => await folderProcessingService.DropAsync(app, path);

        // Then
        await act.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        folderServiceMock.Verify(x => x.GetByPathWithRoles(app.Id, path.Lowered, false), Times.Once);
        folderServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }
}

