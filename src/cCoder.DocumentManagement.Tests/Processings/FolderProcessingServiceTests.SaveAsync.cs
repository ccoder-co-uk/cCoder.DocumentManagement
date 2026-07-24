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
using DmsPath = cCoder.DocumentManagement.Dependencies.Path;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FolderProcessingServiceTests
{
    [Fact]
    public async Task ShouldCreateMissingFolderPathWhenUserCanCreateInAppForSaveAsync()
    {
        // Given
        App app = CreateRandomAppForTests();
        Guid appRoleId = Guid.NewGuid();

        currentUser = new User
        {
            Id = "test-user",
            Roles =
            [
                new UserRole
                {
                    UserId = "test-user",
                    RoleId = appRoleId,
                    Role = new Role
                    {
                        Id = appRoleId,
                        AppId = app.Id,
                        Privileges = ["app_admin", "folder_create"],
                    },
                },
            ],
        };

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(valueFunction: () => currentUser);

        DmsPath path = new(path: "docs/nested");
        Folder createdRoot = CreateRandomFolder();
        createdRoot.AppId = app.Id;
        createdRoot.Name = "docs";
        createdRoot.Path = "docs";
        createdRoot.Roles = [new FolderRole { FolderId = createdRoot.Id, RoleId = appRoleId }];

        Folder createdChild = CreateRandomFolder();
        createdChild.AppId = app.Id;
        createdChild.ParentId = createdRoot.Id;
        createdChild.Name = "nested";
        createdChild.Path = "docs/nested";
        Folder submittedChild = null;

        Role appRole = new()
        {
            Id = appRoleId,
            AppId = app.Id,
            Privileges = ["folder_create"],
        };

        folderServiceMock
            .Setup(expression: x => x.GetByPathWithRoles(appId: app.Id, path: "docs/nested", ignoreFilters: true))
            .Returns(value: (Folder)null);

        folderServiceMock
            .Setup(expression: x => x.GetByPathWithRoles(appId: app.Id, path: "docs", ignoreFilters: true))
            .Returns(value: (Folder)null);

        roleServiceMock.Setup(expression: x => x.GetAll(ignoreFilters: true))
            .Returns(value: new[] { appRole }.AsQueryable());

        folderServiceMock
            .Setup(expression: x => x.AddForPathBuildFolderAsync(newFolder: It.Is<Folder>(match: f => f.Path == "docs" && f.ParentId == null)))
            .ReturnsAsync(value: createdRoot);

        folderServiceMock
            .Setup(expression: x =>
                x.AddForPathBuildFolderAsync(newFolder: It.Is<Folder>(match: f => f.Path == "docs/nested"))
            )
            .Callback<Folder>(action: folder => submittedChild = folder)
            .ReturnsAsync(value: createdChild);

        // When
        await folderProcessingService.SaveAppPathAsync(appId: app.Id, path: path);

        // Then
        folderServiceMock.Verify(expression: x => x.GetByPathWithRoles(appId: app.Id, path: "docs/nested", ignoreFilters: true), times: Times.Once);
        folderServiceMock.Verify(expression: x => x.GetByPathWithRoles(appId: app.Id, path: "docs", ignoreFilters: true), times: Times.Once);
        roleServiceMock.Verify(expression: x => x.GetAll(ignoreFilters: true), times: Times.Once);

        folderServiceMock.Verify(
            expression: x => x.AddForPathBuildFolderAsync(newFolder: It.Is<Folder>(match: f => f.Path == "docs" && f.ParentId == null)),
            times: Times.Once
        );

        folderServiceMock.Verify(
            expression: x => x.AddForPathBuildFolderAsync(newFolder: It.Is<Folder>(match: f => f.Path == "docs/nested")),
            times: Times.Once
        );

        submittedChild.Should()
            .NotBeNull();

        submittedChild.ParentId.Should()
            .Be(expected: createdRoot.Id);

        submittedChild.Roles.Should()
            .ContainSingle();

        submittedChild.Roles.Single().RoleId.Should()
            .Be(expected: appRole.Id);

        authorizationBrokerMock.Verify(expression: x => x.GetCurrentUser(), times: Times.AtLeastOnce());
        folderServiceMock.VerifyNoOtherCalls();
        roleServiceMock.VerifyNoOtherCalls();
        fileServiceMock.VerifyNoOtherCalls();
        fileContentServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldCreateChildFolderWithInheritedRolesWhenParentAllowsCreateForSaveAsync()
    {
        // Given
        Guid roleId = Guid.NewGuid();

        currentUser = new User
        {
            Id = "test-user",
            Roles =
            [
                new UserRole
                {
                    UserId = "test-user",
                    RoleId = roleId,
                    Role = new Role
                    {
                        Id = roleId,
                        AppId = 1,
                        Privileges = ["folder_create"],
                    },
                },
            ],
        };

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(valueFunction: () => currentUser);

        App app = CreateRandomAppForTests();
        DmsPath path = new(path: "docs/nested");
        Folder parentFolder = CreateRandomFolder();
        parentFolder.AppId = app.Id;
        parentFolder.Name = "docs";
        parentFolder.Path = "docs";

        parentFolder.Roles =
        [
            new FolderRole
            {
                FolderId = parentFolder.Id,
                RoleId = roleId,
                Role = new Role
                {
                    Id = roleId,
                    AppId = app.Id,
                    Privileges = ["folder_create"],
                },
            },
        ];

        Folder createdChild = CreateRandomFolder();
        createdChild.AppId = app.Id;
        createdChild.ParentId = parentFolder.Id;
        createdChild.Name = "nested";
        createdChild.Path = "docs/nested";

        folderServiceMock
            .Setup(expression: x => x.GetByPathWithRoles(appId: app.Id, path: "docs/nested", ignoreFilters: true))
            .Returns(value: (Folder)null);

        folderServiceMock
            .Setup(expression: x => x.GetByPathWithRoles(appId: app.Id, path: "docs", ignoreFilters: true))
            .Returns(value: parentFolder);

        folderServiceMock
            .Setup(expression: x =>
                x.AddForPathBuildFolderAsync(
                    newFolder: It.Is<Folder>(match: f =>
                        f.Path == "docs/nested"
                        && f.ParentId == parentFolder.Id
                        && f.Roles.Single().RoleId == roleId)
                )
            )
            .ReturnsAsync(value: createdChild);

        // When
        await folderProcessingService.SaveAppPathAsync(appId: app.Id, path: path);

        // Then
        folderServiceMock.Verify(expression: x => x.GetByPathWithRoles(appId: app.Id, path: "docs/nested", ignoreFilters: true), times: Times.Once);
        folderServiceMock.Verify(expression: x => x.GetByPathWithRoles(appId: app.Id, path: "docs", ignoreFilters: true), times: Times.Once);
        roleServiceMock.VerifyNoOtherCalls();

        folderServiceMock.Verify(
            expression: x =>
                x.AddForPathBuildFolderAsync(
                    newFolder: It.Is<Folder>(match: f =>
                        f.Path == "docs/nested"
                        && f.ParentId == parentFolder.Id
                        && f.Roles.Single().RoleId == roleId)
                ),
            times: Times.Once
        );

        authorizationBrokerMock.Verify(expression: x => x.GetCurrentUser(), times: Times.AtLeastOnce());
        folderServiceMock.VerifyNoOtherCalls();
        fileServiceMock.VerifyNoOtherCalls();
        fileContentServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserCannotCreateFolderPathForSaveAsync()
    {
        // Given
        currentUser = ToLocalUser(user: TestUsers.WithoutPrivileges());

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(valueFunction: () => currentUser);

        App app = CreateRandomAppForTests();
        DmsPath path = new(path: "docs");

        folderServiceMock
            .Setup(expression: x => x.GetByPathWithRoles(appId: app.Id, path: path.Lowered, ignoreFilters: true))
            .Returns(value: (Folder)null);

        // When
        Func<Task> act = async () => await folderProcessingService.SaveAppPathAsync(appId: app.Id, path: path);

        // Then
        await act.Should()
            .ThrowAsync<DocumentManagementServiceException>()
            .WithInnerException(innerException: typeof(SecurityException));

        folderServiceMock.Verify(expression: x => x.GetByPathWithRoles(appId: app.Id, path: path.Lowered, ignoreFilters: true), times: Times.Once);
        roleServiceMock.VerifyNoOtherCalls();
        folderServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.GetCurrentUser(), times: Times.AtLeastOnce());
        authorizationBrokerMock.VerifyNoOtherCalls();
    }
}