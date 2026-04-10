using System.Text;
using cCoder.Data.Models.DMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.DocumentManagement;

public sealed partial class FileContentControllerTests
{
    [Fact]
    public async Task Put_UpdatesFileContent()
    {
        // Given
        SeededFileContentContext seededContext = await SeedDatabase("filecontent_create", "filecontent_update", "filecontent_delete");
        FileContent createdFileContent = await CreateFileContentAsync(new
        {
            fileId = seededContext.FileId,
            description = "Acceptance content",
            size = "4",
            version = 1,
            rawData = Convert.ToBase64String(Encoding.UTF8.GetBytes("test")),
        });
        FileContent actualFileContent;

        // When
        await UpdateFileContentAsync(createdFileContent.Id, new
        {
            id = createdFileContent.Id,
            fileId = seededContext.FileId,
            description = "Updated content",
            size = "8",
            version = 2,
            rawData = Convert.ToBase64String(Encoding.UTF8.GetBytes("updated")),
        });

        actualFileContent = await GetFileContentAsync(createdFileContent.Id);

        // Then
        actualFileContent.Should().NotBeNull();
        actualFileContent!.Version.Should().Be(2);

        await DeleteFileContentAsync(createdFileContent.Id);
        await Teardown(seededContext);
    }
}





