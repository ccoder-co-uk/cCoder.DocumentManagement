using FluentAssertions;
using Xunit;
using DmsFile = cCoder.Data.Models.DMS.File;


namespace Web.AcceptanceTests.Tests.DocumentManagement;

public sealed partial class FileControllerTests
{
    [Fact]
    public async Task Post_CreatesFile()
    {
        // Given
        SeededFileContext seededContext = await SeedDatabase("file_create", "file_delete");
        string name = Unique("File");
        DmsFile expectedFile;
        DmsFile actualFile;

        // When
        expectedFile = await CreateFileAsync(new
        {
            folderId = seededContext.FolderId,
            name,
            description = "Acceptance file",
            path = $"{name}.txt".ToLowerInvariant(),
            mimeType = "text/plain",
            size = "12",
        });

        actualFile = await GetFileAsync(expectedFile.Id);

        // Then
        actualFile.Should().NotBeNull();
        actualFile!.Name.Should().Be(name);

        await DeleteFileAsync(expectedFile.Id);
        await Teardown(seededContext);
    }
}





