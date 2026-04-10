using System.Net;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.DocumentManagement;

public sealed partial class DmsMiddlewareTests
{
    [Fact]
    public async Task Invoke_HandlesOptionsRequest()
    {
        // Given
        (int StatusCode, IReadOnlyCollection<string> Headers) actualResult;

        // When
        actualResult = await InvokeOptionsAsync();

        // Then
        actualResult.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        actualResult.Headers.Should().Contain("Access-Control-Allow-Origin");
        actualResult.Headers.Should().Contain("Access-Control-Allow-Methods");
    }
}



