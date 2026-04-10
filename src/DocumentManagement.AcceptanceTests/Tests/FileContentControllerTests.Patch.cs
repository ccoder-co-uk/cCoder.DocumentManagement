using cCoder.Data.Models.DMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.DocumentManagement;

public sealed partial class FileContentControllerTests
{
    [Fact]
    public async Task Patch_UpdatesFileContent()
    {
        // Given
        SeededFileContentContext seededContext = await SeedDatabase("filecontent_create", "filecontent_update", "filecontent_delete");
        FileContent createdFileContent = await CreateFileContentAsync(new
        {
            fileId = seededContext.FileId,
            description = "Acceptance content",
            size = "4",
            version = 1,
            rawData = new byte[] { 1, 2, 3, 4 },
        });
        FileContent actualFileContent;

        // When
        await PatchFileContentAsync(createdFileContent.Id, new
        {
            description = "Patched content",
            version = 3,
        });

        actualFileContent = await GetFileContentAsync(createdFileContent.Id);

        // Then
        actualFileContent.Should().NotBeNull();
        actualFileContent!.Version.Should().Be(3);

        await DeleteFileContentAsync(createdFileContent.Id);
        await Teardown(seededContext);
    }
}





