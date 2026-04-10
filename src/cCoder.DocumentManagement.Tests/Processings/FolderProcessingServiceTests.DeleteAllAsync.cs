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
        User actor = ToLocalUser(TestUsers.WithPrivilege("app_admin", 1));
        currentUser = actor;
        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(() => currentUser);
        Folder folder = CreateRandomFolder();
        folderServiceMock.Setup(x => x.GetWithRoles(folder.Id, true)).Returns(folder);
        folderServiceMock.Setup(x => x.DeleteAsync(folder.Id)).Returns(ValueTask.CompletedTask);

        // When
        await folderProcessingService.DeleteAllAsync(new[] { folder });

        // Then
        folderServiceMock.Verify(x => x.GetWithRoles(folder.Id, true), Times.Once);
        folderServiceMock.Verify(x => x.DeleteAsync(folder.Id), Times.Once);
        folderServiceMock.VerifyNoOtherCalls();
        loggerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.GetCurrentUser(), Times.AtLeastOnce);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}










