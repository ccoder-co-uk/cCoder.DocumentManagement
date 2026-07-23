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


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FolderProcessingServiceTests
{
    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenAddAsync()
    {
        // Given
        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: It.IsAny<int?>(), privilege: It.IsAny<string>()))
            .Callback(action: (int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId: appId, operation: privilege) ?? false))
                {
                    throw new SecurityException(message: "Access Denied!");
                }
            });

        authorizationBrokerMock
            .Setup(expression: x => x.IsAdminOfApp(appId: It.IsAny<int>()))
            .Returns(valueFunction: (int appId) => currentUser?.IsAdminOfApp(appId: appId) ?? false);

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(valueFunction: () => currentUser);

        Folder folder = new()
        {
            AppId = 1,
            Name = "Child",
            ParentId = Guid.NewGuid(),
        };

        folderServiceMock.Setup(expression: x => x.Get(id: It.IsAny<Guid>()))
            .Returns(value: (Folder)null!);

        // When
        Func<Task> act = async () => await folderProcessingService.AddAsync(newFolder: folder);

        // Then
        await act.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        folderServiceMock.Verify(expression: x => x.Get(id: folder.ParentId.Value), times: Times.Once);
        folderServiceMock.VerifyNoOtherCalls();
        loggerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldReturnExistingFolderWhenMatchingFolderAlreadyExistsForAddAsync()
    {
        // Given
        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: It.IsAny<int?>(), privilege: It.IsAny<string>()))
            .Callback(action: (int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId: appId, operation: privilege) ?? false))
                {
                    throw new SecurityException(message: "Access Denied!");
                }
            });

        authorizationBrokerMock
            .Setup(expression: x => x.IsAdminOfApp(appId: It.IsAny<int>()))
            .Returns(valueFunction: (int appId) => currentUser?.IsAdminOfApp(appId: appId) ?? false);

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(valueFunction: () => currentUser);

        Folder existingFolder = CreateRandomFolder();
        existingFolder.Name = "child";
        existingFolder.Path = "child";
        Folder newFolder = new() { AppId = existingFolder.AppId, Name = existingFolder.Name };

        folderServiceMock.Setup(expression: x => x.GetAll(ignoreFilters: true))
            .Returns(value: new[] { existingFolder }.AsQueryable());

        // When
        Folder result = await folderProcessingService.AddAsync(newFolder: newFolder);

        // Then
        result.Should()
            .BeSameAs(expected: existingFolder);

        folderServiceMock.Verify(expression: x => x.GetAll(ignoreFilters: true), times: Times.Once);
        folderServiceMock.VerifyNoOtherCalls();
        loggerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldSaveAndReturnCreatedFolderWhenAddAsync()
    {
        // Then
        // Given
        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: It.IsAny<int?>(), privilege: It.IsAny<string>()))
            .Callback(action: (int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId: appId, operation: privilege) ?? false))
                {
                    throw new SecurityException(message: "Access Denied!");
                }
            });

        authorizationBrokerMock
            .Setup(expression: x => x.IsAdminOfApp(appId: It.IsAny<int>()))
            .Returns(valueFunction: (int appId) => currentUser?.IsAdminOfApp(appId: appId) ?? false);

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(valueFunction: () => currentUser);

        User actor = ToLocalUser(user: TestUsers.WithPrivileges(privileges: ["app_admin", "folder_create"], appId: 1));
        currentUser = actor;
        App app = CreateRandomAppForTests();
        Folder createdFolder = CreateRandomFolder();
        createdFolder.AppId = app.Id;
        createdFolder.Path = "child";
        createdFolder.Name = "Child";
        Folder folder = new() { AppId = app.Id, Name = "Child" };

        folderServiceMock
            .SetupSequence(expression: x => x.GetAll(ignoreFilters: true))
            .Returns(value: Array.Empty<Folder>()
            .AsQueryable())
            .Returns(value: new[] { createdFolder }.AsQueryable());

        folderServiceMock
            .Setup(expression: x => x.GetByPathWithRoles(appId: app.Id, path: "child", ignoreFilters: true))
            .Returns(value: (Folder)null);

        folderServiceMock
            .Setup(expression: x => x.AddForPathBuildAsync(folder: It.IsAny<Folder>()))
            .ReturnsAsync(value: createdFolder);
        // When
        Folder result = await folderProcessingService.AddAsync(newFolder: folder);

        // Then
        result.Should()
            .BeSameAs(expected: createdFolder);

        folderServiceMock.Verify(expression: x => x.GetAll(ignoreFilters: true), times: Times.Exactly(callCount: 2));
        folderServiceMock.Verify(expression: x => x.GetByPathWithRoles(appId: app.Id, path: "child", ignoreFilters: true), times: Times.Once);
        folderServiceMock.Verify(expression: x => x.AddForPathBuildAsync(folder: It.IsAny<Folder>()), times: Times.Once);
        loggerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldSaveUsingParentPathWhenAddAsync()
    {
        // When

        // Then
        // Given
        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: It.IsAny<int?>(), privilege: It.IsAny<string>()))
            .Callback(action: (int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId: appId, operation: privilege) ?? false))
                {
                    throw new SecurityException(message: "Access Denied!");
                }
            });

        authorizationBrokerMock
            .Setup(expression: x => x.IsAdminOfApp(appId: It.IsAny<int>()))
            .Returns(valueFunction: (int appId) => currentUser?.IsAdminOfApp(appId: appId) ?? false);

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(valueFunction: () => currentUser);

        User actor = ToLocalUser(user: TestUsers.WithPrivileges(privileges: ["app_admin", "folder_create"], appId: 1));
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

        folderServiceMock.Setup(expression: x => x.Get(id: parent.Id))
            .Returns(value: parent);

        folderServiceMock
            .SetupSequence(expression: x => x.GetAll(ignoreFilters: true))
            .Returns(value: Array.Empty<Folder>()
            .AsQueryable())
            .Returns(value: new[] { createdFolder }.AsQueryable());

        folderServiceMock
            .Setup(expression: x => x.GetByPathWithRoles(appId: app.Id, path: "parent/child", ignoreFilters: true))
            .Returns(value: (Folder)null);

        folderServiceMock
            .Setup(expression: x => x.GetByPathWithRoles(appId: app.Id, path: "parent", ignoreFilters: true))
            .Returns(value: (Folder)null);

        folderServiceMock
            .Setup(expression: x => x.AddForPathBuildAsync(folder: It.IsAny<Folder>()))
            .ReturnsAsync(value: createdFolder);
        // When
        Folder result = await folderProcessingService.AddAsync(newFolder: folder);

        // Then
        result.Should()
            .BeSameAs(expected: createdFolder);

        folderServiceMock.Verify(expression: x => x.Get(id: parent.Id), times: Times.Once);
        folderServiceMock.Verify(expression: x => x.GetAll(ignoreFilters: true), times: Times.Exactly(callCount: 2));
        folderServiceMock.Verify(expression: x => x.GetByPathWithRoles(appId: app.Id, path: "parent/child", ignoreFilters: true), times: Times.Once);
        folderServiceMock.Verify(expression: x => x.GetByPathWithRoles(appId: app.Id, path: "parent", ignoreFilters: true), times: Times.Once);
        folderServiceMock.Verify(expression: x => x.AddForPathBuildAsync(folder: It.IsAny<Folder>()), times: Times.Exactly(callCount: 2));
        loggerMock.VerifyNoOtherCalls();
    }

}