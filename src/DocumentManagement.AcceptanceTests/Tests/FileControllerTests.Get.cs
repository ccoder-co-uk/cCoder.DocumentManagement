using FluentAssertions;
using cCoder.Data;
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
        actualCount.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task Get_ReturnsListOfFiles()
    {
        // Given

        // When
        IReadOnlyList<DmsFile> actualFiles = await GetFilesAsync(1);

        // Then
        actualFiles.Should().NotBeNull();
    }

    [Fact]
    public async Task Get_ReturnsFileById()
    {
        // Given
        SeededFileContext seededContext = await SeedDatabase("file_create", "file_delete");
        string name = Unique("File");
        DmsFile expectedFile = await CreateFileAsync(new
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
        actualFile = await GetFileAsync(expectedFile.Id);

        // Then
        actualFile.Should().NotBeNull();
        actualFile!.Id.Should().Be(expectedFile.Id);
        actualFile.Name.Should().Be(name);

        await DeleteFileAsync(expectedFile.Id);
        await Teardown(seededContext);
    }

    [Fact]
    public async Task Get_WithoutReadPrivilege_ReturnsNotFound()
    {
        SeededFileContext seededContext = await SeedDatabase();

        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        cCoder.Data.Models.DMS.Folder hiddenFolder = await core.AddFolderAsync(new cCoder.Data.Models.DMS.Folder
        {
            AppId = seededContext.AppId,
            Name = Unique("HiddenFolder"),
            Path = Unique("hidden-folder").ToLowerInvariant(),
        });

        DmsFile hiddenFile = await core.AddDmsFileAsync(new DmsFile
        {
            FolderId = hiddenFolder.Id,
            Name = Unique("HiddenFile"),
            Description = "Hidden file",
            Path = $"{Unique("hidden")}.txt".ToLowerInvariant(),
            MimeType = "text/plain",
            Size = "12",
        });

        DmsFile actualFile = await GetFileAsync(hiddenFile.Id);

        actualFile.Should().BeNull();

        await Teardown(seededContext);
    }
}






