// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.DocumentManagement.Services.Processings;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;
using App = cCoder.Data.Models.CMS.App;


namespace cCoder.Core.Services.Tests.DMS.Orchestrations;

public partial class DmsHttpRequestOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDelegateToWebDavProcessingServiceWhenRequestIsWebDavForProcessRequestAsync()
    {
        // Given
        App app = CreateApp();
        HttpContext context = CreateContext(method: "GET", path: "/api/webdav/Core/App(7)/DAV/folder/file.txt");

        DmsProcessingResponse response = CreateResponse(
            statusCode: 207,
            contentType: "text/xml; charset=\"utf-8\""
        );

        currentAppResolverMock.Setup(expression: x => x.ResolveCurrentApp())
            .Returns(value: app);

        webDavProcessingServiceMock
            .Setup(expression: x =>
                x.ProcessDmsProcessingSessionAsync(
                    session: It.Is<DmsProcessingSession>(match: session =>
                        session.Request.App.Id == app.Id
                        && session.Request.App.Domain == app.Domain
                        && session.Request.App.Name == app.Name
                        && session.Request.Method == "GET"
                        && session.Request.RequestPath == "/api/webdav/Core/App(7)/DAV/folder/file.txt"
                        && session.Request.Host == "example.test"
                    )
                )
            )
            .ReturnsAsync(value: new DmsProcessingSession { Response = response });

        // When
        await orchestrationService.ProcessRequestAsync(context: context);

        // Then
        context.Response.StatusCode.Should()
            .Be(expected: response.StatusCode);

        context.Response.ContentType.Should()
            .Be(expected: response.ContentType);

        currentAppResolverMock.Verify(expression: x => x.ResolveCurrentApp(), times: Times.Once);

        webDavProcessingServiceMock.Verify(
            expression: x => x.ProcessDmsProcessingSessionAsync(session: It.IsAny<DmsProcessingSession>()),
            times: Times.Once
        );

        currentAppResolverMock.VerifyNoOtherCalls();
        webDavProcessingServiceMock.VerifyNoOtherCalls();
        dmsProcessingServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldDelegateToDmsInstanceProcessingServiceAndAddDefaultHeadersWhenRequestIsDmsForProcessRequestAsync()
    {
        // Given
        App app = CreateApp();

        HttpContext context = CreateContext(
            method: "GET",
            path: "/api/dms/folder/file.txt",
            queryString: "?version=3"
        );

        DmsProcessingResponse response = CreateResponse(statusCode: 200, headers: []);

        currentAppResolverMock.Setup(expression: x => x.ResolveCurrentApp())
            .Returns(value: app);

        dmsProcessingServiceMock
            .Setup(expression: x =>
                x.ProcessDmsProcessingSessionAsync(
                    session: It.Is<DmsProcessingSession>(match: session =>
                        session.Request.App.Id == app.Id
                        && session.Request.App.Domain == app.Domain
                        && session.Request.App.Name == app.Name
                        && session.Request.Method == "GET"
                        && session.Request.RequestPath == "/api/dms/folder/file.txt"
                        && session.Request.QueryString == "?version=3"
                        && session.Request.Host == "example.test"
                    )
                )
            )
            .ReturnsAsync(value: new DmsProcessingSession { Response = response });

        // When
        await orchestrationService.ProcessRequestAsync(context: context);

        // Then
        context.Response.StatusCode.Should()
            .Be(expected: 200);

        context.Response.Headers["Access-Control-Allow-Origin"].ToString()
            .Should()
            .Be(expected: "*");

        context.Response.Headers.ContainsKey(key: "Cache-Control")
            .Should()
            .BeTrue();

        currentAppResolverMock.Verify(expression: x => x.ResolveCurrentApp(), times: Times.Once);

        dmsProcessingServiceMock.Verify(
            expression: x => x.ProcessDmsProcessingSessionAsync(session: It.IsAny<DmsProcessingSession>()),
            times: Times.Once
        );

        currentAppResolverMock.VerifyNoOtherCalls();
        dmsProcessingServiceMock.VerifyNoOtherCalls();
        webDavProcessingServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldNotDuplicateItWhenDmsResponseAlreadyContainsDefaultHeaderForProcessRequestAsync()
    {
        // Given
        App app = CreateApp();
        HttpContext context = CreateContext(method: "GET", path: "/api/dms/folder/file.txt");

        DmsProcessingResponse response = CreateResponse(
            headers:
            [
                new KeyValuePair<string, string>(key:"Access-Control-Allow-Origin", value:"custom-origin"),
            ]
        );

        currentAppResolverMock.Setup(expression: x => x.ResolveCurrentApp())
            .Returns(value: app);

        dmsProcessingServiceMock
            .Setup(expression: x => x.ProcessDmsProcessingSessionAsync(session: It.IsAny<DmsProcessingSession>()))
            .ReturnsAsync(value: new DmsProcessingSession { Response = response });

        // When
        await orchestrationService.ProcessRequestAsync(context: context);

        // Then
        context.Response.Headers["Access-Control-Allow-Origin"].Count
            .Should()
            .Be(expected: 1);

        context.Response.Headers["Access-Control-Allow-Origin"].ToString()
            .Should()
            .Be(expected: "custom-origin");

        currentAppResolverMock.Verify(expression: x => x.ResolveCurrentApp(), times: Times.Once);

        dmsProcessingServiceMock.Verify(
            expression: x => x.ProcessDmsProcessingSessionAsync(session: It.IsAny<DmsProcessingSession>()),
            times: Times.Once
        );

        currentAppResolverMock.VerifyNoOtherCalls();
        dmsProcessingServiceMock.VerifyNoOtherCalls();
        webDavProcessingServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldReturnSecurityResponseWhenDmsProcessingThrowsSecurityExceptionForProcessRequestAsync()
    {
        // Given
        App app = CreateApp();
        HttpContext context = CreateContext(method: "GET", path: "/api/dms/folder/file.txt", host: "tenant.test");

        currentAppResolverMock.Setup(expression: x => x.ResolveCurrentApp())
            .Returns(value: app);

        dmsProcessingServiceMock
            .Setup(expression: x => x.ProcessDmsProcessingSessionAsync(session: It.IsAny<DmsProcessingSession>()))
            .ThrowsAsync(exception: new SecurityException(message: "Denied"));

        // When
        await orchestrationService.ProcessRequestAsync(context: context);

        // Then
        context.Response.StatusCode.Should()
            .Be(expected: 204);

        context.Response.ContentType.Should()
            .Be(expected: "application/json");

        context.Response.Headers["Access-Control-Allow-Origin"].ToString()
            .Should()
            .Be(expected: "tenant.test");

        currentAppResolverMock.Verify(expression: x => x.ResolveCurrentApp(), times: Times.Once);

        dmsProcessingServiceMock.Verify(
            expression: x => x.ProcessDmsProcessingSessionAsync(session: It.IsAny<DmsProcessingSession>()),
            times: Times.Once
        );

        currentAppResolverMock.VerifyNoOtherCalls();
        dmsProcessingServiceMock.VerifyNoOtherCalls();
        webDavProcessingServiceMock.VerifyNoOtherCalls();
    }

}