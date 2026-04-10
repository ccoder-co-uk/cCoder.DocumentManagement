using System.Text;
using cCoder.Data.Models.DMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.DocumentManagement;

public sealed partial class FileContentControllerTests
{
    [Fact]
    public async Task Post_CreatesFileContent()
    {
        // Given
        SeededFileContentContext seededContext = await SeedDatabase("filecontent_create", "filecontent_delete");
        FileContent expectedFileContent;
        FileContent actualFileContent;

        // When
        expectedFileContent = await CreateFileContentAsync(new
        {
            fileId = seededContext.FileId,
            description = "Acceptance content",
            size = "4",
            version = 1,
            rawData = Convert.ToBase64String(Encoding.UTF8.GetBytes("test")),
        });

        actualFileContent = await GetFileContentAsync(expectedFileContent.Id);

        // Then
        actualFileContent.Should().NotBeNull();
        actualFileContent!.Id.Should().Be(expectedFileContent.Id);

        await DeleteFileContentAsync(expectedFileContent.Id);
        await Teardown(seededContext);
    }
}





