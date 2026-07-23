// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using FluentAssertions;
using Web.AcceptanceTests.Infrastructure;
using Xunit;

namespace Web.AcceptanceTests.Tests.DocumentManagement;

[Collection(WebAcceptanceCollection.Name)]
public sealed partial class WebShellTests(WebAcceptanceFixture fixture)
{
    private HttpClient Client { get; } = fixture.Client;

    private async Task<string> GetOkContentAsync(string path)
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: path);
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(expected: HttpStatusCode.OK, because: content);

        return content;
    }
}