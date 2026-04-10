using FluentAssertions;
using Xunit;
using DmsFile = cCoder.Data.Models.DMS.File;


namespace Web.AcceptanceTests.Tests.DocumentManagement;

public sealed partial class FileControllerTests
{
    [Fact]
    public async Task Patch_UpdatesFile()
    {
        // Given
        SeededFileContext seededContext = await SeedDatabase("file_create", "file_update", "file_delete");
        DmsFile createdFile = await CreateFileAsync(new
        {
            folderId = seededContext.FolderId,
            name = Unique("File"),
            description = "Acceptance file",
            path = "file.txt",
            mimeType = "text/plain",
            size = "12",
        });
        string updatedName = Unique("PatchedFile");
        DmsFile actualFile;

        // When
        await PatchFileAsync(createdFile.Id, new
        {
            name = updatedName,
            size = "36",
        });

        actualFile = await GetFileAsync(createdFile.Id);

        // Then
        actualFile.Should().NotBeNull();
        actualFile!.Name.Should().Be(updatedName);
        actualFile.Size.Should().Be("36");

        await DeleteFileAsync(createdFile.Id);
        await Teardown(seededContext);
    }
}





