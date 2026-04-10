using cCoder.Data.Models.DMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.DocumentManagement;

public sealed partial class FolderControllerTests
{
    [Fact]
    public async Task Post_CreatesFolder()
    {
        // Given
        SeededFolderContext seededContext = await SeedDatabase("folder_create", "folder_delete");
        string name = Unique("Folder");
        Folder expectedFolder;
        Folder actualFolder;

        // When
        expectedFolder = await CreateFolderAsync(new
        {
            appId = seededContext.AppId,
            name,
            path = name.ToLowerInvariant(),
        });

        actualFolder = await GetFolderAsync(expectedFolder.Id);

        // Then
        actualFolder.Should().NotBeNull();
        actualFolder!.Name.Should().Be(name);

        await DeleteFolderAsync(expectedFolder.Id);
        await Teardown(seededContext);
    }
}





