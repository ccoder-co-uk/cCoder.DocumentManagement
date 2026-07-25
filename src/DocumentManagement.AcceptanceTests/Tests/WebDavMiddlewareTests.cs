// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        using HttpRequestMessage request = new(method: HttpMethod.Options, requestUri: $"{BaseUrl}/Core/App(1)/DAV/");
        request.Headers.TryAddWithoutValidation(name: "Authorization", value: "Basic acceptance");

        using HttpResponseMessage response = await Client.SendAsync(request: request);
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .NotBe(unexpected: HttpStatusCode.NotFound, because: content);

        response.StatusCode.Should()
            .NotBe(unexpected: HttpStatusCode.MethodNotAllowed, because: content);

        return (int)response.StatusCode;
    }
}