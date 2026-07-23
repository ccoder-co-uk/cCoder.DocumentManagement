// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using Xunit;

namespace Web.AcceptanceTests.Tests.DocumentManagement;

public sealed partial class WebShellTests
{
    [Fact]
    public async Task GetHealth_ReturnsOk()
    {
        // Given

        // When
        string content = await GetOkContentAsync(path: "/Health");

        // Then
        content.Should()
            .Be(expected: "OK");
    }
}