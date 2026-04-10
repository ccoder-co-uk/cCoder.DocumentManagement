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

        User actor = ToLocalUser(TestUsers.WithPrivilege("app_admin", 1));
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
        folderServiceMock.Setup(x => x.GetForUpdate(folder.Id, true)).Returns(dbFolder);
        folderServiceMock.Setup(x => x.GetAll()).Returns(new[] { dbFolder }.AsQueryable());
        folderServiceMock.Setup(x => x.UpdateAsync(It.IsAny<Folder>())).ReturnsAsync((Folder item) => item);
        // When
        Folder result = await folderProcessingService.UpdateAsync(folder);

        // Then
        result.Should().NotBeNull();
        result.Id.Should().Be(folder.Id);
        result.AppId.Should().Be(folder.AppId);
        result.Name.Should().Be(folder.Name);
        result.Path.Should().Be(folder.Path);
        result.ParentId.Should().Be(folder.ParentId);
        folderServiceMock.Verify(x => x.GetForUpdate(folder.Id, true), Times.Once);
        folderServiceMock.Verify(x => x.GetAll(), Times.Once);
        folderServiceMock.Verify(x => x.UpdateAsync(It.IsAny<Folder>()), Times.Once);
        folderServiceMock.VerifyNoOtherCalls();
        loggerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.GetCurrentUser(), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserCannotUpdateFolderForUpdateAsync()
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

        currentUser = ToLocalUser(TestUsers.WithoutPrivileges());
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
        folderServiceMock.Setup(x => x.GetForUpdate(folder.Id, true)).Returns(dbFolder);

        // When
        Func<Task> act = async () => await folderProcessingService.UpdateAsync(folder);

        // Then
        await act.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        folderServiceMock.Verify(x => x.GetForUpdate(folder.Id, true), Times.Once);
        folderServiceMock.VerifyNoOtherCalls();
        loggerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.GetCurrentUser(), Times.Exactly(2));
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}











