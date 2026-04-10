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
        currentAppResolverMock = new Mock<IDocumentManagementCurrentAppResolver>(MockBehavior.Strict);
        dmsProcessingServiceMock = new Mock<IDmsProcessingService>(MockBehavior.Strict);
        webDavProcessingServiceMock = new Mock<IWebDavProcessingService>(MockBehavior.Strict);

        orchestrationService = new DmsHttpRequestOrchestrationService(
            currentAppResolverMock.Object,
            dmsProcessingServiceMock.Object,
            webDavProcessingServiceMock.Object
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
                headerDictionary.Append(key, values);
        }

        requestMock.SetupGet(x => x.Method).Returns(method);
        requestMock.SetupGet(x => x.Path).Returns(new PathString(path));
        requestMock.SetupGet(x => x.Host).Returns(new HostString(host));
        requestMock.SetupGet(x => x.QueryString).Returns(new QueryString(queryString));
        requestMock.SetupGet(x => x.Body).Returns(new MemoryStream(body ?? []));
        requestMock.SetupGet(x => x.Headers).Returns(headerDictionary);
        requestMock.SetupGet(x => x.ContentType).Returns(contentType);

        contextMock.SetupGet(x => x.Request).Returns(requestMock.Object);

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
            Body = hasBody ? new MemoryStream([1, 2, 3]) : Stream.Null,
            Headers = headers ?? [],
        };
}








