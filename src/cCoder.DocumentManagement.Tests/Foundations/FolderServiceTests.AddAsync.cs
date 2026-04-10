using System.Security;
using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;
using DataFolder = cCoder.Data.Models.DMS.Folder;


namespace cCoder.Core.Services.Tests.DMS.Foundations;

public partial class FolderServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForAddAsync()
    {
        // Given
        Folder folder = CreateRandomFolder(appId: 7);

        Folder submitted = null;

        folderBrokerMock.Setup(x => x.GetAppId(It.IsAny<DataFolder>())).Returns((int?)7);

        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "Folder_create"));

        folderBrokerMock
            .Setup(x =>
                x.AddFolderAsync(It.Is<DataFolder>(candidate => candidate.Id != folder.Id))
            )
            .Callback<DataFolder>(candidate =>
                submitted = new Folder
                {
                    Id = candidate.Id,
                    AppId = candidate.AppId,
                    ParentId = candidate.ParentId,
                    Name = candidate.Name,
                    Path = candidate.Path,
                    DeletedOn = candidate.DeletedOn,
                }
            )
            .ReturnsAsync((DataFolder value) => value);

        // When
        Folder result = await folderService.AddAsync(folder);

        // Then
        result.Should().NotBeSameAs(folder);
        submitted.Should().NotBeNull();

        submitted
            .Should()
            .BeEquivalentTo(folder, options => options.Excluding(candidate => candidate.Id));

        result
            .Should()
            .BeEquivalentTo(folder, options => options.Excluding(candidate => candidate.Id));

        folderBrokerMock.Verify(
            x => x.AddFolderAsync(It.Is<DataFolder>(candidate => candidate.Id != folder.Id)),
            Times.Once
        );
        folderBrokerMock.Verify(x => x.GetAppId(It.IsAny<DataFolder>()), Times.AtMostOnce());
        folderBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "Folder_create"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        Folder folder = CreateRandomFolder(appId: 7);

        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "Folder_create"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await folderService.AddAsync(folder);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        folderBrokerMock.Verify(x => x.GetAppId(It.IsAny<DataFolder>()), Times.AtMostOnce());
        folderBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "Folder_create"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}








