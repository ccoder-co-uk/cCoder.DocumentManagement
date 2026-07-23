// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;


using Web.AcceptanceTests.Infrastructure;
namespace Web.AcceptanceTests.Tests.DocumentManagement;

public sealed partial class FolderControllerTests
{
    [Fact]
    public async Task GetCount_ReturnsNonNegativeCount()
    {
        // Given

        // When
        int actualCount = await GetFolderCountAsync();

        // Then
        actualCount.Should().BeGreaterThanOrEqualTo(expected: 0);
    }

    [Fact]
    public async Task Get_ReturnsListOfFolders()
    {
        // Given

        // When
        IReadOnlyList<Folder> actualFolders = await GetFoldersAsync(top: 1);

        // Then
        actualFolders.Should().NotBeNull();
    }

    [Fact]
    public async Task Get_ReturnsFolderById()
    {
        // Given
        SeededFolderContext seededContext = await SeedDatabase("folder_create", "folder_delete");
        string name = Unique(prefix: "Folder");
        Folder expectedFolder = await CreateFolderAsync(payload: new
        {
            appId = seededContext.AppId,
            name,
            path = name.ToLowerInvariant(),
        });
        Folder actualFolder;

        // When
        actualFolder = await GetFolderAsync(id: expectedFolder.Id);

        // Then
        actualFolder.Should().NotBeNull();
        actualFolder!.Id.Should().Be(expected: expectedFolder.Id);
        actualFolder.Name.Should().Be(expected: name);

        await DeleteFolderAsync(id: expectedFolder.Id);
        await Teardown(seededContext: seededContext);
    }

    [Fact]
    public async Task Get_WithoutReadPrivilege_ReturnsNotFound()
    {
        SeededFolderContext seededContext = await SeedDatabase();

        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        App hiddenApp = await core.AddAppAsync(app: new App
        {
            Name = Unique("HiddenApp"),
            Domain = $"{Unique("hidden")}.local",
            DefaultTheme = "Default",
            DefaultCultureId = string.Empty,
            TenantId = Unique("tenant"),
            ConfigJson = "{}",
        });

        Folder hiddenFolder = await core.AddFolderAsync(folder: new Folder
        {
            AppId = hiddenApp.Id,
            Name = Unique("HiddenFolder"),
            Path = Unique("hidden-folder").ToLowerInvariant(),
        });

        Folder actualFolder = await GetFolderAsync(id: hiddenFolder.Id);

        actualFolder.Should().BeNull();

        core.Remove(entity: hiddenFolder);
        core.Remove(entity: hiddenApp);
        await core.SaveChangesAsync();
        await Teardown(seededContext: seededContext);
    }
}