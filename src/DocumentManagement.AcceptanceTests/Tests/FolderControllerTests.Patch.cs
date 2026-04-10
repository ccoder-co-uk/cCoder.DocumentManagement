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
        Folder createdFolder = await CreateFolderAsync(new
        {
            appId = seededContext.AppId,
            name = Unique("Folder"),
            path = "folder",
        });
        string updatedName = Unique("PatchedFolder");
        Folder actualFolder;

        // When
        await PatchFolderAsync(createdFolder.Id, new
        {
            name = updatedName,
            path = "patchedfolder",
        });

        actualFolder = await GetFolderAsync(createdFolder.Id);

        // Then
        actualFolder.Should().NotBeNull();
        actualFolder!.Name.Should().Be(updatedName);

        await DeleteFolderAsync(createdFolder.Id);
        await Teardown(seededContext);
    }
}





