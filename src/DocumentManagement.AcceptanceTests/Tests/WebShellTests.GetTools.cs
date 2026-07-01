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
        string content = await GetOkContentAsync("/tools/index.html");

        // Then
        content.Should().Contain("Document Management");
        content.Should().Contain("folder-grid");
        content.Should().Contain("/tools/api.js");
        content.Should().Contain("/tools/grids.js");
        content.Should().Contain("/tools/styles.css");
    }

    [Fact]
    public async Task GetToolsScripts_ReturnsAggregateAwareGridLogic()
    {
        // Given

        // When
        string content = await GetOkContentAsync("/tools/grids.js");

        // Then
        content.Should().Contain("DocumentManagementGrids");
        content.Should().Contain("data-child-grid=\"File\"");
        content.Should().Contain("data-child-grid=\"FolderRole\"");
        content.Should().Contain("data-child-grid=\"FileContent\"");
        content.Should().Contain("loadFolderDetails");
        content.Should().Contain("loadFileDetails");
    }

    [Fact]
    public async Task GetToolsStyles_ReturnsGridShellStyles()
    {
        // Given

        // When
        string content = await GetOkContentAsync("/tools/styles.css");

        // Then
        content.Should().Contain(".dm-table");
        content.Should().Contain(".dm-detail");
        content.Should().Contain(".dm-tab-panel");
    }
}
