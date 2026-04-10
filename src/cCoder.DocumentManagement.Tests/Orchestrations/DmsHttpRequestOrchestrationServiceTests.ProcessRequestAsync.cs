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
        HttpContext context = CreateContext("GET", "/api/webdav/Core/App(7)/DAV/folder/file.txt");
        DmsProcessingResponse response = CreateResponse(
            statusCode: 207,
            contentType: "text/xml; charset=\"utf-8\""
        );
        currentAppResolverMock.Setup(x => x.ResolveCurrentApp()).Returns(app);

        webDavProcessingServiceMock
            .Setup(x =>
                x.ProcessAsync(
                    It.Is<DmsProcessingRequest>(request =>
                        request.App.Id == app.Id
                        && request.App.Domain == app.Domain
                        && request.App.Name == app.Name
                        && request.Method == "GET"
                        && request.RequestPath == "/api/webdav/Core/App(7)/DAV/folder/file.txt"
                        && request.Host == "example.test"
                    )
                )
            )
            .ReturnsAsync(response);

        // When
        DmsProcessingResponse returnedResponse = await orchestrationService.ProcessRequestAsync(
            context
        );

        // Then
        returnedResponse.Should().BeSameAs(response);
        currentAppResolverMock.Verify(x => x.ResolveCurrentApp(), Times.Once);
        webDavProcessingServiceMock.Verify(
            x => x.ProcessAsync(It.IsAny<DmsProcessingRequest>()),
            Times.Once
        );
        currentAppResolverMock.VerifyNoOtherCalls();
        webDavProcessingServiceMock.VerifyNoOtherCalls();
        dmsProcessingServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldDelegateToDmsProcessingServiceAndAddDefaultHeadersWhenRequestIsDmsForProcessRequestAsync()
    {
        // Given
        App app = CreateApp();
        HttpContext context = CreateContext(
            "GET",
            "/api/dms/folder/file.txt",
            queryString: "?version=3"
        );
        DmsProcessingResponse response = CreateResponse(statusCode: 200, headers: []);
        currentAppResolverMock.Setup(x => x.ResolveCurrentApp()).Returns(app);

        dmsProcessingServiceMock
            .Setup(x =>
                x.ProcessAsync(
                    It.Is<DmsProcessingRequest>(request =>
                        request.App.Id == app.Id
                        && request.App.Domain == app.Domain
                        && request.App.Name == app.Name
                        && request.Method == "GET"
                        && request.RequestPath == "/api/dms/folder/file.txt"
                        && request.QueryString == "?version=3"
                        && request.Host == "example.test"
                    )
                )
            )
            .ReturnsAsync(response);

        // When
        DmsProcessingResponse returnedResponse = await orchestrationService.ProcessRequestAsync(
            context
        );

        // Then
        returnedResponse.StatusCode.Should().Be(200);

        returnedResponse
            .Headers.Should()
            .Contain(header => header.Key == "Access-Control-Allow-Origin" && header.Value == "*");

        returnedResponse.Headers.Should().Contain(header => header.Key == "Cache-Control");
        currentAppResolverMock.Verify(x => x.ResolveCurrentApp(), Times.Once);
        dmsProcessingServiceMock.Verify(
            x => x.ProcessAsync(It.IsAny<DmsProcessingRequest>()),
            Times.Once
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
        HttpContext context = CreateContext("GET", "/api/dms/folder/file.txt");
        DmsProcessingResponse response = CreateResponse(
            headers:
            [
                new KeyValuePair<string, string>("Access-Control-Allow-Origin", "custom-origin"),
            ]
        );
        currentAppResolverMock.Setup(x => x.ResolveCurrentApp()).Returns(app);

        dmsProcessingServiceMock
            .Setup(x => x.ProcessAsync(It.IsAny<DmsProcessingRequest>()))
            .ReturnsAsync(response);

        // When
        DmsProcessingResponse returnedResponse = await orchestrationService.ProcessRequestAsync(
            context
        );

        // Then
        returnedResponse
            .Headers.Count(header => header.Key == "Access-Control-Allow-Origin")
            .Should()
            .Be(1);

        returnedResponse
            .Headers.Should()
            .Contain(header =>
                header.Key == "Access-Control-Allow-Origin" && header.Value == "custom-origin"
            );

        currentAppResolverMock.Verify(x => x.ResolveCurrentApp(), Times.Once);
        dmsProcessingServiceMock.Verify(
            x => x.ProcessAsync(It.IsAny<DmsProcessingRequest>()),
            Times.Once
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
        HttpContext context = CreateContext("GET", "/api/dms/folder/file.txt", host: "tenant.test");
        currentAppResolverMock.Setup(x => x.ResolveCurrentApp()).Returns(app);

        dmsProcessingServiceMock
            .Setup(x => x.ProcessAsync(It.IsAny<DmsProcessingRequest>()))
            .ThrowsAsync(new SecurityException("Denied"));

        // When
        DmsProcessingResponse returnedResponse = await orchestrationService.ProcessRequestAsync(
            context
        );

        // Then
        returnedResponse.StatusCode.Should().Be(204);
        returnedResponse.ContentType.Should().Be("application/json");

        returnedResponse
            .Headers.Should()
            .Contain(header =>
                header.Key == "Access-Control-Allow-Origin" && header.Value == "tenant.test"
            );

        currentAppResolverMock.Verify(x => x.ResolveCurrentApp(), Times.Once);
        dmsProcessingServiceMock.Verify(
            x => x.ProcessAsync(It.IsAny<DmsProcessingRequest>()),
            Times.Once
        );
        currentAppResolverMock.VerifyNoOtherCalls();
        dmsProcessingServiceMock.VerifyNoOtherCalls();
        webDavProcessingServiceMock.VerifyNoOtherCalls();
    }

}






