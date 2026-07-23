// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        actualStatusCode.Should().NotBe(unexpected: (int)HttpStatusCode.NotFound);
        actualStatusCode.Should().NotBe(unexpected: (int)HttpStatusCode.MethodNotAllowed);
    }
}