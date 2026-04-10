using cCoder.Data.Models.DMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.DocumentManagement;

public sealed partial class FolderControllerTests
{
    [Fact]
    public async Task Copy_CopiesFolderBetweenApps()
    {
        // Given
        SeededFolderContext sourceContext = await SeedCopyDatabase("folder_create", "folder_copy", "folder_delete");
        SeededFolderContext destinationContext = await SeedCopyDatabase("folder_create", "folder_copy", "folder_delete");
        string sourceName = Unique("SourceFolder");

        Folder sourceFolder = await CreateFolderAsync(new
        {
            appId = sourceContext.AppId,
            name = sourceName,
            path = sourceName.ToLowerInvariant(),
        });

        string destinationName = "copiedfolder";
        Folder destinationFolder = await CreateFolderAsync(new
        {
            appId = destinationContext.AppId,
            name = destinationName,
            path = destinationName,
        });

        // When
        int actualStatusCode = await CopyFolderAsync(
            sourceName.ToLowerInvariant(),
            destinationName,
            sourceContext.AppId,
            destinationContext.AppId);

        // Then
        actualStatusCode.Should().Be(200);

        await DeleteFolderAsync(sourceFolder.Id);
        await DeleteFolderAsync(destinationFolder.Id);
        await Teardown(sourceContext);
        await Teardown(destinationContext);
    }
}





