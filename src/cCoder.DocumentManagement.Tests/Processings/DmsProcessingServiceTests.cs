// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.DocumentManagement.Brokers.Loggings;
using cCoder.DocumentManagement.Brokers;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Foundations;
using cCoder.DocumentManagement.Services.Processings;
using cCoder.DocumentManagement.Services.Aggregations;
using cCoder.DocumentManagement.Exposures;
using Moq;
using MemoryStream = System.IO.MemoryStream;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class DmsInstanceProcessingServiceTests
{
    private readonly Mock<IDmsInstanceOperationsExposure> dmsInstanceServiceMock;
    private readonly Mock<ILoggingBroker> loggingBrokerMock;
    private readonly DmsRequestAggregationService dmsProcessingService;

    public DmsInstanceProcessingServiceTests()
    {
        dmsInstanceServiceMock = new Mock<IDmsInstanceOperationsExposure>(behavior: MockBehavior.Strict);
        dmsInstanceServiceMock = new();
        loggingBrokerMock = new();
        dmsProcessingService = new DmsRequestAggregationService(
            dmsOperationsExposure: dmsInstanceServiceMock.Object,
            streamBroker: new StreamBroker(),
            log: loggingBrokerMock.Object
        );
    }

    private static App CreateApp() =>
        new()
        {
            Id = 7,
            Domain = "example.test",
            Name = "Example App",
            Roles = [],
            Folders = [],
        };

    private static DmsProcessingRequest CreateRequest(
        string method,
        string requestPath = "/api/dms/folder/file.txt",
        string queryString = "",
        Stream body = null
    ) =>
        new()
        {
            App = CreateApp(),
            Method = method,
            RequestPath = requestPath,
            Host = "example.test",
            QueryString = queryString,
            Body = body ?? new MemoryStream(buffer: []),
        };

    private static byte[] ReadAllBytes(Stream stream)
    {
        stream.Position = 0;
        using MemoryStream copyStream = new();
        stream.CopyTo(destination: copyStream);
        return copyStream.ToArray();
    }
}