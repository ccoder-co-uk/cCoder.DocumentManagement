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
        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(() => currentUser);

        DmsPath path = new("docs/nested");
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
            .Setup(x => x.GetByPathWithRoles(app.Id, "docs/nested", true))
            .Returns((Folder)null);
        folderServiceMock
            .Setup(x => x.GetByPathWithRoles(app.Id, "docs", true))
            .Returns((Folder)null);
        roleServiceMock.Setup(x => x.GetAll(true)).Returns(new[] { appRole }.AsQueryable());
        folderServiceMock
            .Setup(x => x.AddForPathBuildAsync(It.Is<Folder>(f => f.Path == "docs" && f.ParentId == null)))
            .ReturnsAsync(createdRoot);
        folderServiceMock
            .Setup(x =>
                x.AddForPathBuildAsync(It.Is<Folder>(f => f.Path == "docs/nested"))
            )
            .Callback<Folder>(folder => submittedChild = folder)
            .ReturnsAsync(createdChild);

        // When
        await folderProcessingService.SaveAsync(app, path);

        // Then
        folderServiceMock.Verify(x => x.GetByPathWithRoles(app.Id, "docs/nested", true), Times.Once);
        folderServiceMock.Verify(x => x.GetByPathWithRoles(app.Id, "docs", true), Times.Once);
        roleServiceMock.Verify(x => x.GetAll(true), Times.Once);
        folderServiceMock.Verify(
            x => x.AddForPathBuildAsync(It.Is<Folder>(f => f.Path == "docs" && f.ParentId == null)),
            Times.Once
        );
        folderServiceMock.Verify(
            x => x.AddForPathBuildAsync(It.Is<Folder>(f => f.Path == "docs/nested")),
            Times.Once
        );
        submittedChild.Should().NotBeNull();
        submittedChild.ParentId.Should().Be(createdRoot.Id);
        submittedChild.Roles.Should().ContainSingle();
        submittedChild.Roles.Single().RoleId.Should().Be(appRole.Id);
        authorizationBrokerMock.Verify(x => x.GetCurrentUser(), Times.AtLeastOnce());
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
        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(() => currentUser);

        App app = CreateRandomAppForTests();
        DmsPath path = new("docs/nested");
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
            .Setup(x => x.GetByPathWithRoles(app.Id, "docs/nested", true))
            .Returns((Folder)null);
        folderServiceMock
            .Setup(x => x.GetByPathWithRoles(app.Id, "docs", true))
            .Returns(parentFolder);
        folderServiceMock
            .Setup(x =>
                x.AddForPathBuildAsync(
                    It.Is<Folder>(f =>
                        f.Path == "docs/nested"
                        && f.ParentId == parentFolder.Id
                        && f.Roles.Single().RoleId == roleId)
                )
            )
            .ReturnsAsync(createdChild);

        // When
        await folderProcessingService.SaveAsync(app, path);

        // Then
        folderServiceMock.Verify(x => x.GetByPathWithRoles(app.Id, "docs/nested", true), Times.Once);
        folderServiceMock.Verify(x => x.GetByPathWithRoles(app.Id, "docs", true), Times.Once);
        roleServiceMock.VerifyNoOtherCalls();
        folderServiceMock.Verify(
            x =>
                x.AddForPathBuildAsync(
                    It.Is<Folder>(f =>
                        f.Path == "docs/nested"
                        && f.ParentId == parentFolder.Id
                        && f.Roles.Single().RoleId == roleId)
                ),
            Times.Once
        );
        authorizationBrokerMock.Verify(x => x.GetCurrentUser(), Times.AtLeastOnce());
        folderServiceMock.VerifyNoOtherCalls();
        fileServiceMock.VerifyNoOtherCalls();
        fileContentServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserCannotCreateFolderPathForSaveAsync()
    {
        // Given
        currentUser = ToLocalUser(TestUsers.WithoutPrivileges());
        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(() => currentUser);

        App app = CreateRandomAppForTests();
        DmsPath path = new("docs");
        folderServiceMock
            .Setup(x => x.GetByPathWithRoles(app.Id, path.Lowered, true))
            .Returns((Folder)null);

        // When
        Func<Task> act = async () => await folderProcessingService.SaveAsync(app, path);

        // Then
        await act.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        folderServiceMock.Verify(x => x.GetByPathWithRoles(app.Id, path.Lowered, true), Times.Once);
        roleServiceMock.VerifyNoOtherCalls();
        folderServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.GetCurrentUser(), Times.AtLeastOnce());
        authorizationBrokerMock.VerifyNoOtherCalls();
    }
}

