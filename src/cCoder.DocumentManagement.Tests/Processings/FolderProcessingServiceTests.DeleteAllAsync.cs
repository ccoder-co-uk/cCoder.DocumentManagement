// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FolderProcessingServiceTests
{
    [Fact]
    public async Task ShouldDeleteEachFolderWhenUserIsAppAdminForDeleteAllAsync()
    {
        // Given
        User actor = ToLocalUser(user: TestUsers.WithPrivilege("app_admin", 1));
        currentUser = actor;
        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser()).Returns(valueFunction: () => currentUser);
        Folder folder = CreateRandomFolder();
        folderServiceMock.Setup(expression: x => x.GetWithRoles(folder.Id, true)).Returns(value: folder);
        folderServiceMock.Setup(expression: x => x.DeleteAsync(folder.Id)).Returns(value: ValueTask.CompletedTask);

        // When
        await folderProcessingService.DeleteAllAsync(items: new[] { folder });

        // Then
        folderServiceMock.Verify(expression: x => x.GetWithRoles(folder.Id, true), times: Times.Once);
        folderServiceMock.Verify(expression: x => x.DeleteAsync(folder.Id), times: Times.Once);
        folderServiceMock.VerifyNoOtherCalls();
        loggerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.GetCurrentUser(), times: Times.AtLeastOnce);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}