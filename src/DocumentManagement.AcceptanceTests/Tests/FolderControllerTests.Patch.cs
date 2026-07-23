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
    public async Task Patch_UpdatesFolder()
    {
        // Given
        SeededFolderContext seededContext = await SeedDatabase("folder_create", "folder_update", "folder_delete");
        Folder createdFolder = await CreateFolderAsync(payload: new
        {
            appId = seededContext.AppId,
            name = Unique("Folder"),
            path = "folder",
        });
        string updatedName = Unique(prefix: "PatchedFolder");
        Folder actualFolder;

        // When
        await PatchFolderAsync(id: createdFolder.Id, payload: new
        {
            name = updatedName,
            path = "patchedfolder",
        });

        actualFolder = await GetFolderAsync(id: createdFolder.Id);

        // Then
        actualFolder.Should().NotBeNull();
        actualFolder!.Name.Should().Be(expected: updatedName);

        await DeleteFolderAsync(id: createdFolder.Id);
        await Teardown(seededContext: seededContext);
    }
}