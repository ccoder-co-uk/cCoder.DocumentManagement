// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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

        DmsFile createdFile = await CreateFileAsync(payload: new
        {
            folderId = seededContext.FolderId,
            name = Unique(prefix: "File"),
            description = "Acceptance file",
            path = "file.txt",
            mimeType = "text/plain",
            size = "12",
        });

        string updatedName = Unique(prefix: "PatchedFile");
        DmsFile actualFile;

        // When
        await PatchFileAsync(id: createdFile.Id, payload: new
        {
            name = updatedName,
            size = "36",
        });

        actualFile = await GetFileAsync(id: createdFile.Id);

        // Then
        actualFile.Should()
            .NotBeNull();

        actualFile!.Name.Should()
            .Be(expected: updatedName);

        actualFile.Size.Should()
            .Be(expected: "36");

        await DeleteFileAsync(id: createdFile.Id);
        await Teardown(seededContext: seededContext);
    }
}