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
    public async Task ShouldUpdateFolderWhenUserIsAppAdminForUpdateAsync()
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

        User actor = ToLocalUser(user: TestUsers.WithPrivilege(privilege: "app_admin", appId: 1));
        currentUser = actor;
        Folder dbFolder = CreateRandomFolder();
        dbFolder.Name = "Docs";
        dbFolder.Path = "Docs";
        dbFolder.SubFolders = [];
        dbFolder.Files = [];
        dbFolder.Roles = [];

        Folder folder = new()
        {
            Id = dbFolder.Id,
            AppId = dbFolder.AppId,
            Name = dbFolder.Name,
            Path = dbFolder.Path,
            ParentId = dbFolder.ParentId,
            Roles = [],
            Files = [],
            SubFolders = [],
        };

        folderServiceMock.Setup(expression: x => x.GetForUpdate(id: folder.Id, ignoreFilters: true))
            .Returns(value: dbFolder);

        folderServiceMock.Setup(expression: x => x.GetAll())
            .Returns(value: new[] { dbFolder }.AsQueryable());

        folderServiceMock.Setup(expression: x => x.UpdateAsync(folder: It.IsAny<Folder>()))
            .ReturnsAsync(valueFunction: (Folder item) => item);
        // When
        Folder result = await folderProcessingService.UpdateAsync(folder: folder);

        // Then
        result.Should()
            .NotBeNull();

        result.Id.Should()
            .Be(expected: folder.Id);

        result.AppId.Should()
            .Be(expected: folder.AppId);

        result.Name.Should()
            .Be(expected: folder.Name);

        result.Path.Should()
            .Be(expected: folder.Path);

        result.ParentId.Should()
            .Be(expected: folder.ParentId);

        folderServiceMock.Verify(expression: x => x.GetForUpdate(id: folder.Id, ignoreFilters: true), times: Times.Once);
        folderServiceMock.Verify(expression: x => x.GetAll(), times: Times.Once);
        folderServiceMock.Verify(expression: x => x.UpdateAsync(folder: It.IsAny<Folder>()), times: Times.Once);
        folderServiceMock.VerifyNoOtherCalls();
        loggerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.GetCurrentUser(), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserCannotUpdateFolderForUpdateAsync()
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

        currentUser = ToLocalUser(user: TestUsers.WithoutPrivileges());
        Folder dbFolder = CreateRandomFolder();
        dbFolder.Name = "Docs";
        dbFolder.Path = "Docs";
        dbFolder.SubFolders = [];
        dbFolder.Files = [];
        dbFolder.Roles = [];

        Folder folder = new()
        {
            Id = dbFolder.Id,
            AppId = dbFolder.AppId,
            Name = dbFolder.Name,
            Path = dbFolder.Path,
            ParentId = dbFolder.ParentId,
            Roles = [],
            Files = [],
            SubFolders = [],
        };

        folderServiceMock.Setup(expression: x => x.GetForUpdate(id: folder.Id, ignoreFilters: true))
            .Returns(value: dbFolder);

        // When
        Func<Task> act = async () => await folderProcessingService.UpdateAsync(folder: folder);

        // Then
        await act.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        folderServiceMock.Verify(expression: x => x.GetForUpdate(id: folder.Id, ignoreFilters: true), times: Times.Once);
        folderServiceMock.VerifyNoOtherCalls();
        loggerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.GetCurrentUser(), times: Times.Exactly(callCount: 2));
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}