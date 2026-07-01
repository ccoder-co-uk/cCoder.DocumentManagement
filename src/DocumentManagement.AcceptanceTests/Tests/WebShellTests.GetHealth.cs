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
        string content = await GetOkContentAsync("/Health");

        // Then
        content.Should().Be("OK");
    }
}
