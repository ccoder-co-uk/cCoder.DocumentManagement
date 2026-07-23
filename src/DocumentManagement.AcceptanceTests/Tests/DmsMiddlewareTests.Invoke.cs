// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        actualResult.StatusCode.Should().Be(expected: (int)HttpStatusCode.NoContent);
        actualResult.Headers.Should().Contain(expected: "Access-Control-Allow-Origin");
        actualResult.Headers.Should().Contain(expected: "Access-Control-Allow-Methods");
    }
}