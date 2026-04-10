using System.Net;
using FluentAssertions;
using Web.AcceptanceTests.Infrastructure;
using Xunit;


namespace Web.AcceptanceTests.Tests.DocumentManagement;

[Collection(WebAcceptanceCollection.Name)]
public sealed partial class WebDavMiddlewareTests(WebAcceptanceFixture fixture)
{
    private HttpClient Client { get; } = fixture.Client;
    private string BaseUrl { get; } = "/Api/WebDav";

    private async Task<int> InvokeOptionsAsync()
    {
        using HttpRequestMessage request = new(HttpMethod.Options, $"{BaseUrl}/Core/App(1)/DAV/");
        request.Headers.TryAddWithoutValidation("Authorization", "Basic acceptance");

        using HttpResponseMessage response = await Client.SendAsync(request);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().NotBe(HttpStatusCode.NotFound, content);
        response.StatusCode.Should().NotBe(HttpStatusCode.MethodNotAllowed, content);
        return (int)response.StatusCode;
    }
}



