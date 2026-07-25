// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services;
using cCoder.DocumentManagement.Services.Orchestrations;
using cCoder.DocumentManagement.Services.Processings;
using Microsoft.AspNetCore.Http;
using Moq;
using MemoryStream = System.IO.MemoryStream;


namespace cCoder.Core.Services.Tests.DMS.Orchestrations;

public partial class DmsHttpRequestOrchestrationServiceTests
{
    private readonly Mock<ICurrentAppResolverProcessingService> currentAppResolverMock;
    private readonly Mock<IDmsInstanceProcessingService> dmsProcessingServiceMock;
    private readonly Mock<IWebDavProcessingService> webDavProcessingServiceMock;
    private readonly DmsHttpRequestOrchestrationService orchestrationService;

    public DmsHttpRequestOrchestrationServiceTests()
    {
        currentAppResolverMock = new Mock<ICurrentAppResolverProcessingService>(behavior: MockBehavior.Strict);
        dmsProcessingServiceMock = new Mock<IDmsInstanceProcessingService>(behavior: MockBehavior.Strict);
        webDavProcessingServiceMock = new Mock<IWebDavProcessingService>(behavior: MockBehavior.Strict);

        orchestrationService = new DmsHttpRequestOrchestrationService(
            currentAppResolver: currentAppResolverMock.Object,
            dmsProcessingService: dmsProcessingServiceMock.Object,
            webDavProcessingService: webDavProcessingServiceMock.Object
        );
    }

    private static App CreateApp(string domain = "example.test") =>
        new()
        {
            Id = 7,
            Domain = domain,
            Name = "Example App",
            Roles = [],
            Folders = [],
        };

    private static HttpContext CreateContext(
        string method,
        string path,
        string host = "example.test",
        string queryString = "",
        string contentType = "application/json",
        byte[] body = null,
        Dictionary<string, string[]> headers = null
    )
    {
        DefaultHttpContext context = new();
        context.Request.Method = method;
        context.Request.Path = new PathString(value: path);
        context.Request.Host = new HostString(value: host);
        context.Request.QueryString = new QueryString(value: queryString);
        context.Request.Body = new MemoryStream(buffer: body ?? []);
        context.Request.ContentType = contentType;
        context.Response.Body = new MemoryStream();

        if (headers != null)
        {
            foreach ((string key, string[] values) in headers)
            {
                context.Request.Headers.Append(key: key, value: values);
            }
        }

        return context;
    }

    private static DmsProcessingResponse CreateResponse(
        int statusCode = 200,
        string contentType = "application/json",
        bool hasBody = false,
        List<KeyValuePair<string, string>> headers = null
    ) =>
        new()
        {
            StatusCode = statusCode,
            ContentType = contentType,
            HasBody = hasBody,
            Body = hasBody ? new MemoryStream(buffer: [1, 2, 3]) : Stream.Null,
            Headers = headers ?? [],
        };
}