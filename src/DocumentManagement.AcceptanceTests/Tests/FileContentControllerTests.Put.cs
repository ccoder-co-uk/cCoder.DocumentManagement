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
    public async Task Put_UpdatesFileContent()
    {
        // Given
        SeededFileContentContext seededContext = await SeedDatabase("filecontent_create", "filecontent_update", "filecontent_delete");

        FileContent createdFileContent = await CreateLocalFileContentAsync(payload: new
        {
            fileId = seededContext.FileId,
            description = "Acceptance content",
            size = "4",
            version = 1,
            rawData = Convert.ToBase64String(inArray: Encoding.UTF8.GetBytes(s: "test")),
        });

        FileContent actualFileContent;

        // When
        await UpdateFileContentAsync(id: createdFileContent.Id, payload: new
        {
            id = createdFileContent.Id,
            fileId = seededContext.FileId,
            description = "Updated content",
            size = "8",
            version = 2,
            rawData = Convert.ToBase64String(inArray: Encoding.UTF8.GetBytes(s: "updated")),
        });

        actualFileContent = await GetFileContentAsync(id: createdFileContent.Id);

        // Then
        actualFileContent.Should()
            .NotBeNull();

        actualFileContent!.Version.Should()
            .Be(expected: 2);

        await DeleteFileContentAsync(id: createdFileContent.Id);
        await Teardown(seededContext: seededContext);
    }
}