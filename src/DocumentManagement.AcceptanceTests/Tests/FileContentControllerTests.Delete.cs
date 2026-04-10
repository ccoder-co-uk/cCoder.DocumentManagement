using cCoder.Data.Models.DMS;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.DocumentManagement;

public sealed partial class FileContentControllerTests
{
    [Fact]
    public async Task Delete_RemovesFileContent()
    {
        // Given
        SeededFileContentContext seededContext = await SeedDatabase("filecontent_create", "filecontent_delete");
        FileContent createdFileContent = await CreateFileContentAsync(new
        {
            fileId = seededContext.FileId,
            description = "Acceptance content",
            size = "4",
            version = 1,
            rawData = new byte[] { 1, 2, 3, 4 },
        });
        int actualReadStatusCode;

        // When
        int actualStatusCode = await DeleteFileContentAsync(createdFileContent.Id);
        actualReadStatusCode = await GetFileContentStatusCodeAsync(createdFileContent.Id);

        // Then
        actualStatusCode.Should().Be(200);
        actualReadStatusCode.Should().Be(404);

        await Teardown(seededContext);
    }
}





