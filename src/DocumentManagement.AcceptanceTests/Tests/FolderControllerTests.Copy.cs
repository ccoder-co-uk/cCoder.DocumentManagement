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
    public async Task Copy_CopiesFolderBetweenApps()
    {
        // Given
        SeededFolderContext sourceContext = await SeedCopyDatabase("folder_create", "folder_copy", "folder_delete");
        SeededFolderContext destinationContext = await SeedCopyDatabase("folder_create", "folder_copy", "folder_delete");
        string sourceName = Unique(prefix: "SourceFolder");

        Folder sourceFolder = await CreateFolderAsync(payload: new
        {
            appId = sourceContext.AppId,
            name = sourceName,
            path = sourceName.ToLowerInvariant(),
        });

        string destinationName = "copiedfolder";
        Folder destinationFolder = await CreateFolderAsync(payload: new
        {
            appId = destinationContext.AppId,
            name = destinationName,
            path = destinationName,
        });

        // When
        int actualStatusCode = await CopyFolderAsync(
            sourcePath: sourceName.ToLowerInvariant(),
            destinationPath: destinationName,
            sourceAppId: sourceContext.AppId,
            destinationAppId: destinationContext.AppId);

        // Then
        actualStatusCode.Should().Be(expected: 200);

        await DeleteFolderAsync(id: sourceFolder.Id);
        await DeleteFolderAsync(id: destinationFolder.Id);
        await Teardown(seededContext: sourceContext);
        await Teardown(seededContext: destinationContext);
    }
}