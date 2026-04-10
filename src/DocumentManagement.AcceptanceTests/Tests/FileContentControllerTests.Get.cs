using System.Text;
using cCoder.Data.Models.DMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.DocumentManagement;

public sealed partial class FileContentControllerTests
{
    [Fact]
    public async Task GetCount_ReturnsNonNegativeCount()
    {
        // Given

        // When
        int actualCount = await GetFileContentCountAsync();

        // Then
        actualCount.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task Get_ReturnsListOfFileContents()
    {
        // Given

        // When
        IReadOnlyList<FileContent> actualFileContents = await GetFileContentsAsync(1);

        // Then
        actualFileContents.Should().NotBeNull();
    }

    [Fact]
    public async Task Get_ReturnsFileContentById()
    {
        // Given
        SeededFileContentContext seededContext = await SeedDatabase("filecontent_create", "filecontent_delete");
        FileContent expectedFileContent = await CreateFileContentAsync(new
        {
            fileId = seededContext.FileId,
            description = "Acceptance content",
            size = "4",
            version = 1,
            rawData = Convert.ToBase64String(Encoding.UTF8.GetBytes("test")),
        });
        FileContent actualFileContent;

        // When
        actualFileContent = await GetFileContentAsync(expectedFileContent.Id);

        // Then
        actualFileContent.Should().NotBeNull();
        actualFileContent!.Id.Should().Be(expectedFileContent.Id);

        await DeleteFileContentAsync(expectedFileContent.Id);
        await Teardown(seededContext);
    }
}





