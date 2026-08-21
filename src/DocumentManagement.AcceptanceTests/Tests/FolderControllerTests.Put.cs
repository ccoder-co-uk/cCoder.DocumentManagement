// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.DMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.DocumentManagement;

public sealed partial class FolderControllerTests
{
    [Fact]
    public async Task Put_UpdatesFolder()
    {
        // Given
        SeededFolderContext seededContext = await SeedDatabase(privileges:["folder_create","folder_update","folder_delete"]);

        Folder createdFolder = await CreateFolderAsync(payload: new
        {
            appId = seededContext.AppId,
            name = Unique(prefix: "Folder"),
            path = "folder",
        });

        string updatedName = Unique(prefix: "UpdatedFolder");
        Folder actualFolder;

        // When
        await UpdateFolderAsync(id: createdFolder.Id, payload: new
        {
            id = createdFolder.Id,
            appId = seededContext.AppId,
            name = updatedName,
            path = "updatedfolder",
        });

        actualFolder = await GetFolderAsync(id: createdFolder.Id);

        // Then
        actualFolder.Should()
            .NotBeNull();

        actualFolder!.Name.Should()
            .Be(expected: updatedName);

        await DeleteFolderAsync(id: createdFolder.Id);
        await Teardown(seededContext: seededContext);
    }

    [Fact]
    public async Task Put_MovesFolderToParent()
    {
        // Given
        SeededFolderContext seededContext = await SeedDatabase(
            privileges: ["folder_create", "folder_update", "folder_delete"]);

        Folder sourceFolder = await CreateFolderAsync(payload: new
        {
            appId = seededContext.AppId,
            name = Unique(prefix: "SourceFolder"),
            path = "sourcefolder",
        });

        Folder targetFolder = await CreateFolderAsync(payload: new
        {
            appId = seededContext.AppId,
            name = Unique(prefix: "TargetFolder"),
            path = "targetfolder",
        });

        // When
        await UpdateFolderAsync(id: sourceFolder.Id, payload: new
        {
            id = sourceFolder.Id,
            appId = seededContext.AppId,
            name = sourceFolder.Name,
            parentId = targetFolder.Id,
            path = $"{targetFolder.Path}/{sourceFolder.Name.ToLowerInvariant()}",
        });

        Folder actualFolder = await GetFolderAsync(id: sourceFolder.Id);

        // Then
        actualFolder.Should()
            .NotBeNull();

        actualFolder!.ParentId.Should()
            .Be(expected: targetFolder.Id);

        await Teardown(seededContext: seededContext);
    }
}