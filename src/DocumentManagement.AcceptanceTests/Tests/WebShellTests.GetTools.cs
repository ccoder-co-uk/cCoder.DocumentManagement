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
        content.Should().Contain("/tools/company-logo.png");
        content.Should().Contain("dm-logo");
        content.Should().Contain("Sign in required");
        content.Should().Contain("dm-login-gate");
        content.Should().Contain("dm-workbench");
        content.Should().Contain("Document Management workspace tabs");
        content.Should().Contain("dm-workspace-tabs");
        content.Should().Contain("folder-grid");
        content.Should().Contain("/tools/api.js");
        content.Should().Contain("/tools/grids.js");
        content.Should().Contain("/tools/styles.css");
    }

    [Fact]
    public async Task GetToolsApi_ReturnsLoginGateLogic()
    {
        // Given

        // When
        string content = await GetOkContentAsync("/tools/api.js");

        // Then
        content.Should().Contain("document-management-auth-changed");
        content.Should().Contain("isAuthenticated: function");
        content.Should().Contain("document.body.classList.toggle(\"is-authenticated\"");
    }

    [Fact]
    public async Task GetToolsScripts_ReturnsAggregateAwareGridLogic()
    {
        // Given

        // When
        string content = await GetOkContentAsync("/tools/grids.js");

        // Then
        content.Should().Contain("DocumentManagementGrids");
        content.Should().Contain("DocumentManagementApi.isAuthenticated()");
        content.Should().Contain("document-management-auth-changed");
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
        content.Should().Contain("body.dm-shell:not(.is-authenticated) .dm-workbench");
        content.Should().Contain("body.dm-shell.is-authenticated .dm-login-gate");
        content.Should().Contain(".dm-logo");
        content.Should().Contain(".dm-workspace-tabs");
        content.Should().Contain(".dm-workspace-tabs button.active");
    }
}
