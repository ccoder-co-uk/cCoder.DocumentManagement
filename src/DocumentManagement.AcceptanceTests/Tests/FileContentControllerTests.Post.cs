// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        expectedFileContent = await CreateFileContentAsync(payload: new
        {
            fileId = seededContext.FileId,
            description = "Acceptance content",
            size = "4",
            version = 1,
            rawData = Convert.ToBase64String(inArray: Encoding.UTF8.GetBytes(s: "test")),
        });

        actualFileContent = await GetFileContentAsync(id: expectedFileContent.Id);

        // Then
        actualFileContent.Should()
            .NotBeNull();

        actualFileContent!.Id.Should()
            .Be(expected: expectedFileContent.Id);

        await DeleteFileContentAsync(id: expectedFileContent.Id);
        await Teardown(seededContext: seededContext);
    }
}