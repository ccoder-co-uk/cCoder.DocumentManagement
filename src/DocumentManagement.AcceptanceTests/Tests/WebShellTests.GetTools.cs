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
        content.Should()
            .Contain(expected: "Document Management");

        content.Should()
            .Contain(expected: "/tools/company-logo.png");

        content.Should()
            .Contain(expected: "dm-logo");

        content.Should()
            .Contain(expected: "Sign in required");

        content.Should()
            .Contain(expected: "dm-login-gate");

        content.Should()
            .Contain(expected: "dm-workbench");

        content.Should()
            .Contain(expected: "Document Management workspace tabs");

        content.Should()
            .Contain(expected: "dm-workspace-tabs");

        content.Should()
            .Contain(expected: "folder-grid");

        content.Should()
            .Contain(expected: "/tools/api.js");

        content.Should()
            .Contain(expected: "/tools/grids.js");

        content.Should()
            .Contain(expected: "/tools/styles.css");
    }

    [Fact]
    public async Task GetToolsApi_ReturnsLoginGateLogic()
    {
        // Given

        // When
        string content = await GetOkContentAsync(path: "/tools/api.js");

        // Then
        content.Should()
            .Contain(expected: "document-management-auth-changed");

        content.Should()
            .Contain(expected: "isAuthenticated: function");

        content.Should()
            .Contain(expected: "document.body.classList.toggle(\"is-authenticated\"");
    }

    [Fact]
    public async Task GetToolsScripts_ReturnsAggregateAwareGridLogic()
    {
        // Given

        // When
        string content = await GetOkContentAsync(path: "/tools/grids.js");

        // Then
        content.Should()
            .Contain(expected: "DocumentManagementGrids");

        content.Should()
            .Contain(expected: "DocumentManagementApi.isAuthenticated()");

        content.Should()
            .Contain(expected: "document-management-auth-changed");

        content.Should()
            .Contain(expected: "data-child-grid=\"File\"");

        content.Should()
            .Contain(expected: "data-child-grid=\"FolderRole\"");

        content.Should()
            .Contain(expected: "data-child-grid=\"FileContent\"");

        content.Should()
            .Contain(expected: "loadFolderDetails");

        content.Should()
            .Contain(expected: "loadFileDetails");
    }

    [Fact]
    public async Task GetToolsStyles_ReturnsGridShellStyles()
    {
        // Given

        // When
        string content = await GetOkContentAsync(path: "/tools/styles.css");

        // Then
        content.Should()
            .Contain(expected: ".dm-table");

        content.Should()
            .Contain(expected: ".dm-detail");

        content.Should()
            .Contain(expected: ".dm-tab-panel");

        content.Should()
            .Contain(expected: "body.dm-shell:not(.is-authenticated) .dm-workbench");

        content.Should()
            .Contain(expected: "body.dm-shell.is-authenticated .dm-login-gate");

        content.Should()
            .Contain(expected: ".dm-logo");

        content.Should()
            .Contain(expected: ".dm-workspace-tabs");

        content.Should()
            .Contain(expected: ".dm-workspace-tabs button.active");
    }
}