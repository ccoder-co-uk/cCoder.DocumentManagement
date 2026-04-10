using System.Net;
using FluentAssertions;
using Web.AcceptanceTests.Infrastructure;
using Xunit;


namespace Web.AcceptanceTests.Tests.DocumentManagement;

[Collection(WebAcceptanceCollection.Name)]
public sealed partial class DmsMiddlewareTests(WebAcceptanceFixture fixture)
{
    private HttpClient Client { get; } = fixture.Client;
    private string BaseUrl { get; } = "/Api/Dms";

    private async Task<(int StatusCode, IReadOnlyCollection<string> Headers)> InvokeOptionsAsync()
    {
        using HttpRequestMessage request = new(HttpMethod.Options, $"{BaseUrl}/test-folder");
        using HttpResponseMessage response = await Client.SendAsync(request);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.NoContent, content);
        return ((int)response.StatusCode, response.Headers.Select(header => header.Key).ToArray());
    }
}



