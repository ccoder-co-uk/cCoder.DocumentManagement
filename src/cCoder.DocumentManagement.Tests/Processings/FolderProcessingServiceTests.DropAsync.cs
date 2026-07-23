// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        currentUser = ToLocalUser(user: TestUsers.WithPrivilege("folder_delete", 1));
        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser()).Returns(valueFunction: () => currentUser);

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
            .Setup(expression: x => x.GetByPathWithRoles(app.Id, folder.Path, false))
            .Returns(value: folder);
        folderServiceMock.Setup(expression: x => x.DeleteAsync(folder.Id)).Returns(value: ValueTask.CompletedTask);

        // When
        await folderProcessingService.DropAsync(app: app, path: new DmsPath(folder.Path));

        // Then
        folderServiceMock.Verify(expression: x => x.GetByPathWithRoles(app.Id, folder.Path, false), times: Times.Once);
        folderServiceMock.Verify(expression: x => x.DeleteAsync(folder.Id), times: Times.Once);
        folderServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.GetCurrentUser(), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenFolderCannotBeDeletedForDropAsync()
    {
        // Given
        currentUser = ToLocalUser(user: TestUsers.WithoutPrivileges());
        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser()).Returns(valueFunction: () => currentUser);

        App app = CreateRandomAppForTests();
        DmsPath path = new(path: "docs");
        folderServiceMock
            .Setup(expression: x => x.GetByPathWithRoles(app.Id, path.Lowered, false))
            .Returns(value: (Folder)null);

        // When
        Func<Task> act = async () => await folderProcessingService.DropAsync(app: app, path: path);

        // Then
        await act.Should().ThrowAsync<SecurityException>().WithMessage(expectedWildcardPattern: "Access Denied!");
        folderServiceMock.Verify(expression: x => x.GetByPathWithRoles(app.Id, path.Lowered, false), times: Times.Once);
        folderServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }
}