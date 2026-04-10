using System.Security;
using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FolderProcessingServiceTests
{
    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenAddAsync()
    {
        // Given
        authorizationBrokerMock
            .Setup(x => x.Authorize(It.IsAny<int?>(), It.IsAny<string>()))
            .Callback((int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId, privilege) ?? false))
                    throw new SecurityException("Access Denied!");
            });

        authorizationBrokerMock
            .Setup(x => x.IsAdminOfApp(It.IsAny<int>()))
            .Returns((int appId) => currentUser?.IsAdminOfApp(appId) ?? false);

        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(() => currentUser);

        Folder folder = new()
        {
            AppId = 1,
            Name = "Child",
            ParentId = Guid.NewGuid(),
        };

        folderServiceMock.Setup(x => x.Get(It.IsAny<Guid>())).Returns((Folder)null!);

        // When
        Func<Task> act = async () => await folderProcessingService.AddAsync(folder);

        // Then
        await act.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        folderServiceMock.Verify(x => x.Get(folder.ParentId.Value), Times.Once);
        folderServiceMock.VerifyNoOtherCalls();
        loggerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldReturnExistingFolderWhenMatchingFolderAlreadyExistsForAddAsync()
    {
        // Given
        authorizationBrokerMock
            .Setup(x => x.Authorize(It.IsAny<int?>(), It.IsAny<string>()))
            .Callback((int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId, privilege) ?? false))
                    throw new SecurityException("Access Denied!");
            });

        authorizationBrokerMock
            .Setup(x => x.IsAdminOfApp(It.IsAny<int>()))
            .Returns((int appId) => currentUser?.IsAdminOfApp(appId) ?? false);

        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(() => currentUser);

        Folder existingFolder = CreateRandomFolder();
        existingFolder.Name = "child";
        existingFolder.Path = "child";
        Folder newFolder = new() { AppId = existingFolder.AppId, Name = existingFolder.Name };

        folderServiceMock.Setup(x => x.GetAll(true)).Returns(new[] { existingFolder }.AsQueryable());

        // When
        Folder result = await folderProcessingService.AddAsync(newFolder);

        // Then
        result.Should().BeSameAs(existingFolder);
        folderServiceMock.Verify(x => x.GetAll(true), Times.Once);
        folderServiceMock.VerifyNoOtherCalls();
        loggerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldSaveAndReturnCreatedFolderWhenAddAsync()
    {
        // Then
        // Given
        authorizationBrokerMock
            .Setup(x => x.Authorize(It.IsAny<int?>(), It.IsAny<string>()))
            .Callback((int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId, privilege) ?? false))
                    throw new SecurityException("Access Denied!");
            });

        authorizationBrokerMock
            .Setup(x => x.IsAdminOfApp(It.IsAny<int>()))
            .Returns((int appId) => currentUser?.IsAdminOfApp(appId) ?? false);

        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(() => currentUser);

        User actor = ToLocalUser(TestUsers.WithPrivileges(["app_admin", "folder_create"], 1));
        currentUser = actor;
        App app = CreateRandomAppForTests();
        Folder createdFolder = CreateRandomFolder();
        createdFolder.AppId = app.Id;
        createdFolder.Path = "child";
        createdFolder.Name = "Child";
        Folder folder = new() { AppId = app.Id, Name = "Child" };

        folderServiceMock
            .SetupSequence(x => x.GetAll(true))
            .Returns(Array.Empty<Folder>().AsQueryable())
            .Returns(new[] { createdFolder }.AsQueryable());
        folderServiceMock
            .Setup(x => x.GetByPathWithRoles(app.Id, "child", true))
            .Returns((Folder)null);
        folderServiceMock
            .Setup(x => x.AddForPathBuildAsync(It.IsAny<Folder>()))
            .ReturnsAsync(createdFolder);
        // When
        Folder result = await folderProcessingService.AddAsync(folder);

        // Then
        result.Should().BeSameAs(createdFolder);
        folderServiceMock.Verify(x => x.GetAll(true), Times.Exactly(2));
        folderServiceMock.Verify(x => x.GetByPathWithRoles(app.Id, "child", true), Times.Once);
        folderServiceMock.Verify(x => x.AddForPathBuildAsync(It.IsAny<Folder>()), Times.Once);
        loggerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldSaveUsingParentPathWhenAddAsync()
    {
        // When

        // Then
        // Given
        authorizationBrokerMock
            .Setup(x => x.Authorize(It.IsAny<int?>(), It.IsAny<string>()))
            .Callback((int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId, privilege) ?? false))
                    throw new SecurityException("Access Denied!");
            });

        authorizationBrokerMock
            .Setup(x => x.IsAdminOfApp(It.IsAny<int>()))
            .Returns((int appId) => currentUser?.IsAdminOfApp(appId) ?? false);

        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(() => currentUser);

        User actor = ToLocalUser(TestUsers.WithPrivileges(["app_admin", "folder_create"], 1));
        currentUser = actor;
        App app = CreateRandomAppForTests();
        Folder parent = CreateRandomFolder();
        parent.AppId = app.Id;
        parent.Name = "parent";
        parent.Path = "parent";
        parent.Roles = [];
        Folder createdFolder = CreateRandomFolder();
        createdFolder.AppId = app.Id;
        createdFolder.Name = "Child";
        createdFolder.Path = "parent/child";
        createdFolder.ParentId = parent.Id;
        Folder folder = new()
        {
            AppId = app.Id,
            Name = "Child",
            ParentId = parent.Id,
        };

        folderServiceMock.Setup(x => x.Get(parent.Id)).Returns(parent);
        folderServiceMock
            .SetupSequence(x => x.GetAll(true))
            .Returns(Array.Empty<Folder>().AsQueryable())
            .Returns(new[] { createdFolder }.AsQueryable());
        folderServiceMock
            .Setup(x => x.GetByPathWithRoles(app.Id, "parent/child", true))
            .Returns((Folder)null);
        folderServiceMock
            .Setup(x => x.GetByPathWithRoles(app.Id, "parent", true))
            .Returns((Folder)null);
        folderServiceMock
            .Setup(x => x.AddForPathBuildAsync(It.IsAny<Folder>()))
            .ReturnsAsync(createdFolder);
        // When
        Folder result = await folderProcessingService.AddAsync(folder);

        // Then
        result.Should().BeSameAs(createdFolder);
        folderServiceMock.Verify(x => x.Get(parent.Id), Times.Once);
        folderServiceMock.Verify(x => x.GetAll(true), Times.Exactly(2));
        folderServiceMock.Verify(x => x.GetByPathWithRoles(app.Id, "parent/child", true), Times.Once);
        folderServiceMock.Verify(x => x.GetByPathWithRoles(app.Id, "parent", true), Times.Once);
        folderServiceMock.Verify(x => x.AddForPathBuildAsync(It.IsAny<Folder>()), Times.Exactly(2));
        loggerMock.VerifyNoOtherCalls();
    }

}











