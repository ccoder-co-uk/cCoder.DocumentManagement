using cCoder.Data.Models.DMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.DocumentManagement;

public sealed partial class FolderControllerTests
{
    [Fact]
    public async Task Delete_RemovesFolder()
    {
        // Given
        SeededFolderContext seededContext = await SeedDatabase("folder_create", "folder_delete");
        Folder createdFolder = await CreateFolderAsync(new
        {
            appId = seededContext.AppId,
            name = Unique("Folder"),
            path = "folder",
        });
        int actualReadStatusCode;

        // When
        int actualStatusCode = await DeleteFolderAsync(createdFolder.Id);
        actualReadStatusCode = await GetFolderStatusCodeAsync(createdFolder.Id);

        // Then
        actualStatusCode.Should().Be(200);
        actualReadStatusCode.Should().Be(404);

        await Teardown(seededContext);
    }
}





