// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
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

public partial class WebDavProcessingServiceTests
{
    private readonly Mock<IFileService> fileServiceMock;
    private readonly Mock<IFolderService> folderServiceMock;
    private readonly Mock<IDmsInstanceService> dmsInstanceServiceMock;
    private readonly Mock<ILogger<WebDavProcessingService>> loggerMock;
    private readonly WebDavProcessingService webDavProcessingService;

    public WebDavProcessingServiceTests()
    {
        fileServiceMock = new Mock<IFileService>(behavior: MockBehavior.Strict);
        folderServiceMock = new Mock<IFolderService>(behavior: MockBehavior.Strict);
        dmsInstanceServiceMock = new Mock<IDmsInstanceService>(behavior: MockBehavior.Strict);
        fileServiceMock = new();
        folderServiceMock = new();
        dmsInstanceServiceMock = new();
        loggerMock = new();

        Config config = new() { Settings = new Dictionary<string, string> { [key: "sslPort"] = "443" } };

        webDavProcessingService = new WebDavProcessingService(
            fileService: fileServiceMock.Object,
            folderService: folderServiceMock.Object,
            dmsInstanceService: dmsInstanceServiceMock.Object,
            config: config,
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
        string requestPath = "Core/App(7)/DAV/folder/file.txt",
        string queryString = "",
        string contentType = "text/plain",
        Stream body = null,
        Dictionary<string, string[]> headers = null
    ) =>
        new()
        {
            App = CreateApp(),
            Method = method,
            RequestPath = requestPath,
            Host = "example.test",
            QueryString = queryString,
            ContentType = contentType,
            Body = body ?? new MemoryStream(buffer: []),
            Headers =
                headers
                ?? new Dictionary<string, string[]>(comparer: StringComparer.OrdinalIgnoreCase)
                {
                    [key: "Authorization"] = ["Basic token"],
                },
        };

    private static string ReadBodyText(Stream stream)
    {
        stream.Position = 0;
        using StreamReader reader = new(stream: stream, leaveOpen: true);
        string text = reader.ReadToEnd();
        stream.Position = 0;
        return text;
    }

    private static byte[] ReadAllBytes(Stream stream)
    {
        stream.Position = 0;
        using MemoryStream copy = new();
        stream.CopyTo(destination: copy);
        stream.Position = 0;
        return copy.ToArray();
    }

    private static Folder CreateFolderAsync(
        string path,
        int appId = 7,
        Guid parentId = default,
        string name = null
    ) =>
        new()
        {
            Id = Guid.NewGuid(),
            AppId = appId,
            ParentId = parentId == Guid.Empty ? null : parentId,
            Name = name ?? (path.Length == 0 ? "Root" : path.Split(separator: '/').Last()),
            Path = path,
            Files = [],
            SubFolders = [],
            Roles = [],
        };

    private static cCoder.Data.Models.DMS.File CreateFileAsync(string path, Folder folder) =>
        new()
        {
            Id = Guid.NewGuid(),
            FolderId = folder.Id,
            Folder = folder,
            Name = path.Split(separator: '/').Last(),
            Path = path,
            MimeType = "text/plain",
            CreatedOn = DateTimeOffset.UtcNow,
            Contents =
            [
                new FileContent
                {
                    Id = Guid.NewGuid(),
                    FileId = Guid.NewGuid(),
                    Version = 1,
                    RawData = [1, 2, 3],
                    CreatedOn = DateTimeOffset.UtcNow,
                },
            ],
        };
}