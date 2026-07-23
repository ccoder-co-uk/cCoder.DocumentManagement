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
    public async Task Post_CreatesFile()
    {
        // Given
        SeededFileContext seededContext = await SeedDatabase("file_create", "file_delete");
        string name = Unique(prefix: "File");
        DmsFile expectedFile;
        DmsFile actualFile;

        // When
        expectedFile = await CreateFileAsync(payload: new
        {
            folderId = seededContext.FolderId,
            name,
            description = "Acceptance file",
            path = $"{name}.txt".ToLowerInvariant(),
            mimeType = "text/plain",
            size = "12",
        });

        actualFile = await GetFileAsync(id: expectedFile.Id);

        // Then
        actualFile.Should().NotBeNull();
        actualFile!.Name.Should().Be(expected: name);

        await DeleteFileAsync(id: expectedFile.Id);
        await Teardown(seededContext: seededContext);
    }
}