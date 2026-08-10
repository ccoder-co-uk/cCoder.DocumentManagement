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

        folderServiceMock
            .Setup(expression: service => service.GetForUpdate(folderId: folder.Id, ignoreFilters: true))
            .Returns(value: null);

        IQueryable<Folder> folders = Queryable.AsQueryable(
            source: Array.Empty<Folder>());

        folderServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(value: folders);

        fileProcessingServiceMock
            .Setup(expression: service => service.SaveAppPathAsync(
                appId: folder.AppId,
                path: It.IsAny<cCoder.DocumentManagement.Dependencies.Path>()))
            .Returns(value: ValueTask.CompletedTask);

        // When
        _ = await folderProcessingService.AddOrUpdateFolder(items: [folder]);

        // Then
        fileProcessingServiceMock.Verify(
            expression: service => service.SaveAppPathAsync(
                appId: folder.AppId,
                path: It.IsAny<cCoder.DocumentManagement.Dependencies.Path>()),
            times: Times.Once);
    }
}