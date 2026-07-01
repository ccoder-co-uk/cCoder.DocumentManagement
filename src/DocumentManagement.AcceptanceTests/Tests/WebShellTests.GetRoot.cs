using System.Net;
using FluentAssertions;
using Xunit;

namespace Web.AcceptanceTests.Tests.DocumentManagement;

public sealed partial class WebShellTests
{
    [Fact]
    public async Task GetRoot_RedirectsToToolsShell()
    {
        // Given

        // When
        using HttpResponseMessage response = await Client.GetAsync("/");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.OriginalString.Should().Be("/tools/index.html");
    }
}
