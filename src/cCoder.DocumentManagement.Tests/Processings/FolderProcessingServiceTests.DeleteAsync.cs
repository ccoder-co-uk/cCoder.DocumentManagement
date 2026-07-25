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
    public async Task ShouldDeleteFolderWhenUserIsAppAdminForDeleteAsync()
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

        User user = ToLocalUser(user: TestUsers.WithPrivilege(privilege: "app_admin", appId: 1));
        currentUser = user;
        Folder folder = CreateRandomFolder();

        folderServiceMock.Setup(expression: x => x.GetWithRoles(folderId: folder.Id, ignoreFilters: true))
            .Returns(value: folder);

        folderServiceMock.Setup(expression: x => x.DeleteAsync(folderId: folder.Id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await folderProcessingService.DeleteAsync(folderId: folder.Id);

        // Then
        folderServiceMock.Verify(expression: x => x.GetWithRoles(folderId: folder.Id, ignoreFilters: true), times: Times.Once);
        folderServiceMock.Verify(expression: x => x.DeleteAsync(folderId: folder.Id), times: Times.Once);
        folderServiceMock.VerifyNoOtherCalls();
        loggerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.GetCurrentUser(), times: Times.AtLeastOnce);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
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

        Folder folder = CreateRandomFolder();
        currentUser = ToLocalUser(user: TestUsers.WithoutPrivileges());

        folderServiceMock.Setup(expression: x => x.GetWithRoles(folderId: folder.Id, ignoreFilters: true))
            .Returns(value: folder);

        // When
        Func<Task> act = async () => await folderProcessingService.DeleteAsync(folderId: folder.Id);

        // Then
        await act.Should()
            .ThrowAsync<DocumentManagementServiceException>()
            .WithInnerException(innerException: typeof(SecurityException));

        folderServiceMock.Verify(expression: x => x.GetWithRoles(folderId: folder.Id, ignoreFilters: true), times: Times.Once);
        folderServiceMock.VerifyNoOtherCalls();
        loggerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.GetCurrentUser(), times: Times.AtLeastOnce);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}