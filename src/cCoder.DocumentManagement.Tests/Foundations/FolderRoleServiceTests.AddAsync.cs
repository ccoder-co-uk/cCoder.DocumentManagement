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

        folderRoleBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<DataFolderRole>()))
            .Returns(value: (int?)7);

        authorizationBrokerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "FolderRole_create"));

        folderRoleBrokerMock
            .Setup(expression: x =>
                x.AddFolderRoleAsync(
                    entity: It.Is<DataFolderRole>(match: candidate =>
                        candidate.FolderId == folderRole.FolderId && candidate.RoleId == folderRole.RoleId
                    )
                )
            )
            .Callback<DataFolderRole>(action: candidate =>
                submitted = new FolderRole { FolderId = candidate.FolderId, RoleId = candidate.RoleId }
            )
            .ReturnsAsync(valueFunction: (DataFolderRole value) => value);

        // When
        FolderRole result = await folderRoleService.AddAsync(folderRole: folderRole);

        // Then
        result.Should()
            .BeSameAs(expected: folderRole);

        submitted.Should()
            .NotBeNull();

        submitted.Should()
            .NotBeSameAs(unexpected: folderRole);

        result.Should()
            .NotBeSameAs(unexpected: submitted);

        submitted.Should()
            .BeEquivalentTo(expectation: folderRole);

        result.Should()
            .BeEquivalentTo(expectation: folderRole);

        folderRoleBrokerMock.Verify(
            expression: x =>
                x.AddFolderRoleAsync(
                    entity: It.Is<DataFolderRole>(match: candidate =>
                        candidate.FolderId == folderRole.FolderId && candidate.RoleId == folderRole.RoleId
                    )
                ),
            times: Times.Once
        );

        folderRoleBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<DataFolderRole>()), times: Times.AtMostOnce());
        folderRoleBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "FolderRole_create"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        FolderRole folderRole = CreateRandomFolderRole();

        folderRoleBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<DataFolderRole>()))
            .Returns(value: (int?)7);

        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "FolderRole_create"))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () => await folderRoleService.AddAsync(folderRole: folderRole);

        // Then
        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        folderRoleBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<DataFolderRole>()), times: Times.AtMostOnce());
        folderRoleBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "FolderRole_create"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}