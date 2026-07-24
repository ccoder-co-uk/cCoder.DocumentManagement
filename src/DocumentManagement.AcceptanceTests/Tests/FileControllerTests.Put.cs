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
    public async Task Put_UpdatesFile()
    {
        // Given
        SeededFileContext seededContext = await SeedDatabase(privileges:["file_create","file_update","file_delete"]);

        DmsFile createdFile = await CreateLocalFileAsync(payload: new
        {
            folderId = seededContext.FolderId,
            name = Unique(prefix: "File"),
            description = "Acceptance file",
            path = "file.txt",
            mimeType = "text/plain",
            size = "12",
        });

        string updatedName = Unique(prefix: "UpdatedFile");
        DmsFile actualFile;

        // When
        await UpdateFileAsync(id: createdFile.Id, payload: new
        {
            id = createdFile.Id,
            folderId = seededContext.FolderId,
            name = updatedName,
            description = "Updated file",
            path = "updated.txt",
            mimeType = "text/plain",
            size = "24",
        });

        actualFile = await GetFileAsync(id: createdFile.Id);

        // Then
        actualFile.Should()
            .NotBeNull();

        actualFile!.Name.Should()
            .Be(expected: updatedName);

        await DeleteFileAsync(id: createdFile.Id);
        await Teardown(seededContext: seededContext);
    }
}