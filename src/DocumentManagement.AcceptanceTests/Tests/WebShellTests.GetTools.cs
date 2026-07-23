// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using Xunit;

namespace Web.AcceptanceTests.Tests.DocumentManagement;

public sealed partial class WebShellTests
{
    [Fact]
    public async Task GetTools_ReturnsDocumentManagementShell()
    {
        // Given

        // When
        string content = await GetOkContentAsync(path: "/tools/index.html");

        // Then
        content.Should().Contain(expected: "Document Management");
        content.Should().Contain(expected: "folder-grid");
        content.Should().Contain(expected: "/tools/api.js");
        content.Should().Contain(expected: "/tools/grids.js");
        content.Should().Contain(expected: "/tools/styles.css");
    }

    [Fact]
    public async Task GetToolsScripts_ReturnsAggregateAwareGridLogic()
    {
        // Given

        // When
        string content = await GetOkContentAsync(path: "/tools/grids.js");

        // Then
        content.Should().Contain(expected: "DocumentManagementGrids");
        content.Should().Contain(expected: "data-child-grid=\"File\"");
        content.Should().Contain(expected: "data-child-grid=\"FolderRole\"");
        content.Should().Contain(expected: "data-child-grid=\"FileContent\"");
        content.Should().Contain(expected: "loadFolderDetails");
        content.Should().Contain(expected: "loadFileDetails");
    }

    [Fact]
    public async Task GetToolsStyles_ReturnsGridShellStyles()
    {
        // Given

        // When
        string content = await GetOkContentAsync(path: "/tools/styles.css");

        // Then
        content.Should().Contain(expected: ".dm-table");
        content.Should().Contain(expected: ".dm-detail");
        content.Should().Contain(expected: ".dm-tab-panel");
    }
}