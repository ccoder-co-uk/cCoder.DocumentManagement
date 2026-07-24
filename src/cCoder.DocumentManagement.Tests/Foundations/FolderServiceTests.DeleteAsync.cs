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
using DataFolder = cCoder.Data.Models.DMS.Folder;


namespace cCoder.Core.Services.Tests.DMS.Foundations;

public partial class FolderServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForDeleteAsync()
    {
        // Given
        Guid folderId = Guid.NewGuid();
        Folder folder = CreateRandomFolder(id: folderId, appId: 7);

        folderBrokerMock
            .Setup(expression: x => x.SelectAllFolders(ignoreFilters: true))
            .Returns(value: new[] { ToExternalFolder(folder: folder) }.AsQueryable());

        authorizationBrokerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "Folder_delete"));

        folderBrokerMock.Setup(expression: x => x.DeleteFolderAsync(deletedFolder: It.IsAny<DataFolder>()))
            .ReturnsAsync(value: 1);

        // When
        await folderService.DeleteAsync(folderId: folderId);

        // Then
        folderBrokerMock.Verify(expression: x => x.SelectAllFolders(ignoreFilters: true), times: Times.Once);

        folderBrokerMock.Verify(
            expression: x => x.DeleteFolderAsync(deletedFolder: It.Is<DataFolder>(match: candidate => candidate.Id == folder.Id)),
            times: Times.Once
        );

        folderBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "Folder_delete"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        Guid folderId = Guid.NewGuid();
        Folder folder = CreateRandomFolder(id: folderId, appId: 7);

        folderBrokerMock
            .Setup(expression: x => x.SelectAllFolders(ignoreFilters: true))
            .Returns(value: new[] { ToExternalFolder(folder: folder) }.AsQueryable());

        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "Folder_delete"))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () => await folderService.DeleteAsync(folderId: folderId);

        // Then
        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        folderBrokerMock.Verify(expression: x => x.SelectAllFolders(ignoreFilters: true), times: Times.Once);
        folderBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "Folder_delete"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}