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
    public async Task GetCount_ReturnsNonNegativeCount()
    {
        // Given

        // When
        int actualCount = await GetFileContentCountAsync();

        // Then
        actualCount.Should()
            .BeGreaterThanOrEqualTo(expected: 0);
    }

    [Fact]
    public async Task Get_ReturnsListOfFileContents()
    {
        // Given

        // When
        IReadOnlyList<FileContent> actualFileContents = await GetFileContentsAsync(top: 1);

        // Then
        actualFileContents.Should()
            .NotBeNull();
    }

    [Fact]
    public async Task Get_ReturnsFileContentById()
    {
        // Given
        SeededFileContentContext seededContext = await SeedDatabase("filecontent_create", "filecontent_delete");

        FileContent expectedFileContent = await CreateLocalFileContentAsync(payload: new
        {
            fileId = seededContext.FileId,
            description = "Acceptance content",
            size = "4",
            version = 1,
            rawData = Convert.ToBase64String(inArray: Encoding.UTF8.GetBytes(s: "test")),
        });

        FileContent actualFileContent;

        // When
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