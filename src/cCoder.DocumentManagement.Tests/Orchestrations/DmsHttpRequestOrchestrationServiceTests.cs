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
    private readonly Mock<IDocumentManagementCurrentAppResolver> currentAppResolverMock;
    private readonly Mock<IDmsProcessingService> dmsProcessingServiceMock;
    private readonly Mock<IWebDavProcessingService> webDavProcessingServiceMock;
    private readonly DmsHttpRequestOrchestrationService orchestrationService;

    public DmsHttpRequestOrchestrationServiceTests()
    {
        currentAppResolverMock = new Mock<IDocumentManagementCurrentAppResolver>(behavior: MockBehavior.Strict);
        dmsProcessingServiceMock = new Mock<IDmsProcessingService>(behavior: MockBehavior.Strict);
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
        Mock<HttpRequest> requestMock = new();
        Mock<HttpContext> contextMock = new();
        HeaderDictionary headerDictionary = [];

        if (headers != null)
        {
            foreach ((string key, string[] values) in headers)
            {
                headerDictionary.Append(key: key, value: values);
            }
        }

        requestMock.SetupGet(expression: x => x.Method)
            .Returns(value: method);

        requestMock.SetupGet(expression: x => x.Path)
            .Returns(value: new PathString(value: path));

        requestMock.SetupGet(expression: x => x.Host)
            .Returns(value: new HostString(value: host));

        requestMock.SetupGet(expression: x => x.QueryString)
            .Returns(value: new QueryString(value: queryString));

        requestMock.SetupGet(expression: x => x.Body)
            .Returns(value: new MemoryStream(buffer: body ?? []));

        requestMock.SetupGet(expression: x => x.Headers)
            .Returns(value: headerDictionary);

        requestMock.SetupGet(expression: x => x.ContentType)
            .Returns(value: contentType);

        contextMock.SetupGet(expression: x => x.Request)
            .Returns(value: requestMock.Object);

        return contextMock.Object;
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