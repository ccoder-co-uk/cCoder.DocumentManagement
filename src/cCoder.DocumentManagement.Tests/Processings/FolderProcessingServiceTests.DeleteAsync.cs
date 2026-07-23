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
            .Setup(expression: x => x.Authorize(It.IsAny<int?>(), It.IsAny<string>()))
            .Callback(action: (int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId, privilege) ?? false))
                {
                    throw new SecurityException("Access Denied!");
                }
            });

        authorizationBrokerMock
            .Setup(expression: x => x.IsAdminOfApp(It.IsAny<int>()))
            .Returns(valueFunction: (int appId) => currentUser?.IsAdminOfApp(appId) ?? false);

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser()).Returns(valueFunction: () => currentUser);

        User user = ToLocalUser(user: TestUsers.WithPrivilege("app_admin", 1));
        currentUser = user;
        Folder folder = CreateRandomFolder();
        folderServiceMock.Setup(expression: x => x.GetWithRoles(folder.Id, true)).Returns(value: folder);
        folderServiceMock.Setup(expression: x => x.DeleteAsync(folder.Id)).Returns(value: ValueTask.CompletedTask);

        // When
        await folderProcessingService.DeleteAsync(id: folder.Id);

        // Then
        folderServiceMock.Verify(expression: x => x.GetWithRoles(folder.Id, true), times: Times.Once);
        folderServiceMock.Verify(expression: x => x.DeleteAsync(folder.Id), times: Times.Once);
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
            .Setup(expression: x => x.Authorize(It.IsAny<int?>(), It.IsAny<string>()))
            .Callback(action: (int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId, privilege) ?? false))
                {
                    throw new SecurityException("Access Denied!");
                }
            });

        authorizationBrokerMock
            .Setup(expression: x => x.IsAdminOfApp(It.IsAny<int>()))
            .Returns(valueFunction: (int appId) => currentUser?.IsAdminOfApp(appId) ?? false);

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser()).Returns(valueFunction: () => currentUser);

        Folder folder = CreateRandomFolder();
        currentUser = ToLocalUser(user: TestUsers.WithoutPrivileges());
        folderServiceMock.Setup(expression: x => x.GetWithRoles(folder.Id, true)).Returns(value: folder);

        // When
        Func<Task> act = async () => await folderProcessingService.DeleteAsync(id: folder.Id);

        // Then
        await act.Should().ThrowAsync<SecurityException>().WithMessage(expectedWildcardPattern: "Access Denied!");
        folderServiceMock.Verify(expression: x => x.GetWithRoles(folder.Id, true), times: Times.Once);
        folderServiceMock.VerifyNoOtherCalls();
        loggerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.GetCurrentUser(), times: Times.AtLeastOnce);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}