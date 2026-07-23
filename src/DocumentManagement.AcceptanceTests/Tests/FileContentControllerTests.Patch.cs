// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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

        FileContent createdFileContent = await CreateFileContentAsync(payload: new
        {
            fileId = seededContext.FileId,
            description = "Acceptance content",
            size = "4",
            version = 1,
            rawData = new byte[] { 1, 2, 3, 4 },
        });

        FileContent actualFileContent;

        // When
        await PatchFileContentAsync(id: createdFileContent.Id, payload: new
        {
            description = "Patched content",
            version = 3,
        });

        actualFileContent = await GetFileContentAsync(id: createdFileContent.Id);

        // Then
        actualFileContent.Should()
            .NotBeNull();

        actualFileContent!.Version.Should()
            .Be(expected: 3);

        await DeleteFileContentAsync(id: createdFileContent.Id);
        await Teardown(seededContext: seededContext);
    }
}