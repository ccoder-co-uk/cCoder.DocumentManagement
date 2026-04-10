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
        SeededFolderContext seededContext = await SeedDatabase("folder_create", "folder_update", "folder_delete");
        Folder createdFolder = await CreateFolderAsync(new
        {
            appId = seededContext.AppId,
            name = Unique("Folder"),
            path = "folder",
        });
        string updatedName = Unique("UpdatedFolder");
        Folder actualFolder;

        // When
        await UpdateFolderAsync(createdFolder.Id, new
        {
            id = createdFolder.Id,
            appId = seededContext.AppId,
            name = updatedName,
            path = "updatedfolder",
        });

        actualFolder = await GetFolderAsync(createdFolder.Id);

        // Then
        actualFolder.Should().NotBeNull();
        actualFolder!.Name.Should().Be(updatedName);

        await DeleteFolderAsync(createdFolder.Id);
        await Teardown(seededContext);
    }
}





