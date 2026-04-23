using System.Security;
using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;
using DataFolderRole = cCoder.Data.Models.Security.FolderRole;


namespace cCoder.Core.Services.Tests.DMS.Foundations;

public partial class FolderRoleServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForAddAsync()
    {
        // Given
        FolderRole folderRole = CreateRandomFolderRole();

        FolderRole submitted = null;

        folderRoleBrokerMock.Setup(x => x.GetAppId(It.IsAny<DataFolderRole>())).Returns((int?)7);
        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "FolderRole_create"));

        folderRoleBrokerMock
            .Setup(x =>
                x.AddFolderRoleAsync(
                    It.Is<DataFolderRole>(candidate =>
                        candidate.FolderId == folderRole.FolderId && candidate.RoleId == folderRole.RoleId
                    )
                )
            )
            .Callback<DataFolderRole>(candidate =>
                submitted = new FolderRole { FolderId = candidate.FolderId, RoleId = candidate.RoleId }
            )
            .ReturnsAsync((DataFolderRole value) => value);

        // When
        FolderRole result = await folderRoleService.AddAsync(folderRole);

        // Then
        result.Should().BeSameAs(folderRole);
        submitted.Should().NotBeNull();
        submitted.Should().NotBeSameAs(folderRole);
        result.Should().NotBeSameAs(submitted);
        submitted.Should().BeEquivalentTo(folderRole);
        result.Should().BeEquivalentTo(folderRole);
        folderRoleBrokerMock.Verify(
            x =>
                x.AddFolderRoleAsync(
                    It.Is<DataFolderRole>(candidate =>
                        candidate.FolderId == folderRole.FolderId && candidate.RoleId == folderRole.RoleId
                    )
                ),
            Times.Once
        );
        folderRoleBrokerMock.Verify(x => x.GetAppId(It.IsAny<DataFolderRole>()), Times.AtMostOnce());
        folderRoleBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "FolderRole_create"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        FolderRole folderRole = CreateRandomFolderRole();

        folderRoleBrokerMock.Setup(x => x.GetAppId(It.IsAny<DataFolderRole>())).Returns((int?)7);
        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "FolderRole_create"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await folderRoleService.AddAsync(folderRole);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        folderRoleBrokerMock.Verify(x => x.GetAppId(It.IsAny<DataFolderRole>()), Times.AtMostOnce());
        folderRoleBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "FolderRole_create"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}








