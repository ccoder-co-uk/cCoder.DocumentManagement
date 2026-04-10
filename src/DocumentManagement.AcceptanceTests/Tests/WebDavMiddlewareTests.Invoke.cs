using System.Net;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.DocumentManagement;

public sealed partial class WebDavMiddlewareTests
{
    [Fact]
    public async Task Invoke_HandlesOptionsRequest()
    {
        // Given
        int actualStatusCode;

        // When
        actualStatusCode = await InvokeOptionsAsync();

        // Then
        actualStatusCode.Should().NotBe((int)HttpStatusCode.NotFound);
        actualStatusCode.Should().NotBe((int)HttpStatusCode.MethodNotAllowed);
    }
}



