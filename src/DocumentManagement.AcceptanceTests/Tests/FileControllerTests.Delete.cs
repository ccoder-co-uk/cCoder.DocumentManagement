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
    public async Task Delete_RemovesFile()
    {
        // Given
        SeededFileContext seededContext = await SeedDatabase("file_create", "file_delete");
        DmsFile createdFile = await CreateFileAsync(payload: new
        {
            folderId = seededContext.FolderId,
            name = Unique("File"),
            description = "Acceptance file",
            path = "file.txt",
            mimeType = "text/plain",
            size = "12",
        });
        int actualReadStatusCode;

        // When
        int actualStatusCode = await DeleteFileAsync(id: createdFile.Id);
        actualReadStatusCode = await GetFileStatusCodeAsync(id: createdFile.Id);

        // Then
        actualStatusCode.Should().Be(expected: 200);
        actualReadStatusCode.Should().Be(expected: 404);

        await Teardown(seededContext: seededContext);
    }
}