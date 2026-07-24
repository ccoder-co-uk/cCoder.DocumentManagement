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
    public async Task Post_CreatesFolder()
    {
        // Given
        SeededFolderContext seededContext = await SeedDatabase(privileges:["folder_create","folder_delete"]);
        string name = Unique(prefix: "Folder");
        Folder expectedFolder;
        Folder actualFolder;

        // When
        expectedFolder = await CreateFolderAsync(payload: new
        {
            appId = seededContext.AppId,
            name,
            path = name.ToLowerInvariant(),
        });

        actualFolder = await GetFolderAsync(id: expectedFolder.Id);

        // Then
        actualFolder.Should()
            .NotBeNull();

        actualFolder!.Name.Should()
            .Be(expected: name);

        await DeleteFolderAsync(id: expectedFolder.Id);
        await Teardown(seededContext: seededContext);
    }
}