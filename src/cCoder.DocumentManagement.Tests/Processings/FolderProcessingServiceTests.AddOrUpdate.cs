// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.DMS;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FolderProcessingServiceTests
{
    [Fact]
    public async Task ShouldAddFolderWhenSuppliedIdDoesNotExistAsync()
    {
        // Given
        Folder folder = CreateRandomFolder();
        Folder createdFolder = CreateRandomFolder();

        currentUser = ToLocalUser(user: TestUsers.WithPrivileges(
            privileges: ["app_admin", "folder_create"],
            appId: folder.AppId));

        authorizationBrokerMock
            .Setup(expression: broker => broker.GetCurrentUser())
            .Returns(valueFunction: () => currentUser);

        authorizationBrokerMock
            .Setup(expression: broker => broker.IsAdminOfApp(appId: folder.AppId))
            .Returns(value: true);

        folderServiceMock
            .Setup(expression: service => service.GetForUpdate(folderId: folder.Id, ignoreFilters: true))
            .Returns(value: null);

        folderServiceMock
            .SetupSequence(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(value: Array.Empty<Folder>()
                .AsQueryable())
            .Returns(value: new[] { createdFolder }
                .AsQueryable());

        folderServiceMock
            .Setup(expression: service => service.GetByPathWithRoles(
                appId: folder.AppId,
                path: folder.Name.ToLowerInvariant(),
                ignoreFilters: true))
            .Returns(value: null);

        folderServiceMock
            .Setup(expression: service => service.AddForPathBuildFolderAsync(
                newFolder: It.IsAny<Folder>()))
            .ReturnsAsync(value: createdFolder);

        // When
        _ = await folderProcessingService.AddOrUpdateFolder(items: [folder]);

        // Then
        folderServiceMock.Verify(
            expression: service => service.AddForPathBuildFolderAsync(
                newFolder: It.IsAny<Folder>()),
            times: Times.Once);
    }
}