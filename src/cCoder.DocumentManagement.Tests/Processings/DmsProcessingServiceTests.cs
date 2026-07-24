// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Foundations;
using cCoder.DocumentManagement.Services.Processings;
using Microsoft.Extensions.Logging;
using Moq;
using MemoryStream = System.IO.MemoryStream;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class DmsInstanceProcessingServiceTests
{
    private readonly Mock<IDmsInstanceService> dmsInstanceServiceMock;
    private readonly Mock<ILogger<DmsInstanceProcessingService>> loggerMock;
    private readonly DmsInstanceProcessingService dmsProcessingService;

    public DmsInstanceProcessingServiceTests()
    {
        dmsInstanceServiceMock = new Mock<IDmsInstanceService>(behavior: MockBehavior.Strict);
        dmsInstanceServiceMock = new();
        loggerMock = new();
        dmsProcessingService = new DmsInstanceProcessingService(
            dmsInstanceService: dmsInstanceServiceMock.Object,
            log: loggerMock.Object
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