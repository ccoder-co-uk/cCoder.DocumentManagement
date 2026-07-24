// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using cCoder.Data;
using cCoder.Data.Models.CMS;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using DmsFile = cCoder.Data.Models.DMS.File;


using Web.AcceptanceTests.Infrastructure;
namespace Web.AcceptanceTests.Tests.DocumentManagement;

public sealed partial class FileControllerTests
{
    [Fact]
    public async Task GetCount_ReturnsNonNegativeCount()
    {
        // Given

        // When
        int actualCount = await GetFileCountAsync();

        // Then
        actualCount.Should()
            .BeGreaterThanOrEqualTo(expected: 0);
    }

    [Fact]
    public async Task Get_ReturnsListOfFiles()
    {
        // Given

        // When
        IReadOnlyList<DmsFile> actualFiles = await GetFilesAsync(top: 1);

        // Then
        actualFiles.Should()
            .NotBeNull();
    }

    [Fact]
    public async Task Get_ReturnsFileById()
    {
        // Given
        SeededFileContext seededContext = await SeedDatabase(privileges:["file_create","file_delete"]);
        string name = Unique(prefix: "File");

        DmsFile expectedFile = await CreateLocalFileAsync(payload: new
        {
            folderId = seededContext.FolderId,
            name,
            description = "Acceptance file",
            path = $"{name}.txt".ToLowerInvariant(),
            mimeType = "text/plain",
            size = "12",
        });

        DmsFile actualFile;

        // When
        actualFile = await GetFileAsync(id: expectedFile.Id);

        // Then
        actualFile.Should()
            .NotBeNull();

        actualFile!.Id.Should()
            .Be(expected: expectedFile.Id);

        actualFile.Name.Should()
            .Be(expected: name);

        await DeleteFileAsync(id: expectedFile.Id);
        await Teardown(seededContext: seededContext);
    }

    [Fact]
    public async Task Get_WithoutReadPrivilege_ReturnsNotFound()
    {
        // Given
        SeededFileContext seededContext = await SeedDatabase();

        using IServiceScope scope = fixture.Factory.Services.CreateScope();

        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        App hiddenApp = await core.AddAppAsync(app: new App
        {
            Name = Unique(prefix: "HiddenApp"),
            Domain = $"{Unique(prefix: "hidden")}.local",
            DefaultTheme = "Default",
            DefaultCultureId = string.Empty,
            TenantId = Unique(prefix: "tenant"),
            ConfigJson = "{}",
        });

        cCoder.Data.Models.DMS.Folder hiddenFolder = await core.InsertFolderAsync(folder: new cCoder.Data.Models.DMS.Folder
        {
            AppId = hiddenApp.Id,
            Name = Unique(prefix: "HiddenFolder"),
            Path = Unique(prefix: "hidden-folder")
            .ToLowerInvariant(),
        });

        DmsFile hiddenFile = await core.AddDmsFileAsync(file: new DmsFile
        {
            FolderId = hiddenFolder.Id,
            Name = Unique(prefix: "HiddenFile"),
            Description = "Hidden file",
            Path = $"{Unique(prefix: "hidden")}.txt".ToLowerInvariant(),
            MimeType = "text/plain",
            Size = "12",
        });

        // When
        DmsFile actualFile = await GetFileAsync(id: hiddenFile.Id);

        // Then
        actualFile.Should()
            .BeNull();

        core.Remove(entity: hiddenFile);
        core.Remove(entity: hiddenFolder);
        core.Remove(entity: hiddenApp);
        await core.SaveChangesAsync();
        await Teardown(seededContext: seededContext);
    }
}