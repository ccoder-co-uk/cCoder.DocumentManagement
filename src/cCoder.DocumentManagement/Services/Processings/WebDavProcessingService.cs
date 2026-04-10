using System.Net;
using System.Text;
using System.Xml.Linq;
using cCoder.Data;
using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Foundations;
using LocalFile = cCoder.Data.Models.DMS.File;
using LocalFolder = cCoder.Data.Models.DMS.Folder;
using LocalPath = cCoder.DocumentManagement.Models.Path;
using DmsResult = cCoder.DocumentManagement.Models.DMSResult;
using MemoryStream = System.IO.MemoryStream;


namespace cCoder.DocumentManagement.Services.Processings;

internal class WebDavProcessingService(
    IFileService fileService,
    IFolderService folderService,
    IDmsInstanceService dmsInstanceService,
    Config config,
    ILogger<WebDavProcessingService> log
) : IWebDavProcessingService
{
    public async ValueTask<DmsProcessingResponse> ProcessAsync(DmsProcessingRequest request)
    {
        int appId = ExtractAppId(request.RequestPath);
        LocalPath path = new(
            WebUtility
                .UrlDecode(NormalizeRequestPath(request.RequestPath, appId))
                .TrimStart('/')
                .TrimEnd('/')
        );
        string requestText = await ReadRequestBodyTextAsync(request.Body);

        log.LogDebug($"HTTP {request.Method.ToUpperInvariant()} - {path} \n {requestText}");

        XNamespace ns = "DAV:";
        App app = request.App;
        Dictionary<string, string[]> query = ParseQuery(request.QueryString);

        string sslPort = config.Settings.TryGetValue("sslPort", out string configuredSslPort)
            ? configuredSslPort
            : "443";
        string urlBase = $"https://{app.Domain}:{sslPort}/Api/";

        List<KeyValuePair<string, string>> headers =
        [
            new KeyValuePair<string, string>("Host", urlBase + "DAV/"),
        ];

        if (!request.Headers.ContainsKey("Authorization"))
        {
            headers.Add(
                new KeyValuePair<string, string>("WWW-Authenticate", "Basic realm=\"server\"")
            );
            headers.Add(new KeyValuePair<string, string>("Connection", "close"));
            return CreateResponse(
                EncodeText(string.Empty),
                true,
                "text/xml; charset=\"utf-8\"",
                401,
                headers
            );
        }

        try
        {
            switch (request.Method.ToUpperInvariant())
            {
                case "OPTIONS":
                    headers.AddRange([
                        new KeyValuePair<string, string>(
                            "Access-Control-Allow-Origin",
                            request.Host
                        ),
                        new KeyValuePair<string, string>(
                            "Allow",
                            "OPTIONS, GET, HEAD, POST, PUT, DELETE, TRACE, COPY, MOVE, MKCOL, PROPFIND, PROPPATCH, LOCK, UNLOCK, ORDERPATCH"
                        ),
                        new KeyValuePair<string, string>(
                            "Public",
                            "OPTIONS, GET, HEAD, POST, PUT, DELETE, TRACE, COPY, MOVE, MKCOL, PROPFIND, PROPPATCH, LOCK, UNLOCK, ORDERPATCH"
                        ),
                        new KeyValuePair<string, string>(
                            "Date",
                            DateTimeOffset.Now.ToString("s") + "Z"
                        ),
                        new KeyValuePair<string, string>("DAV", "1, 2"),
                        new KeyValuePair<string, string>("MS-Author-Via", "DAV"),
                    ]);

                    return CreateResponse(
                        Stream.Null,
                        false,
                        "text/xml; charset=\"utf-8\"",
                        204,
                        headers
                    );
                case "GET":
                    int getVer = int.TryParse(
                        TryGetSingleValue(query, "version"),
                        out int parsedVersion
                    )
                        ? parsedVersion
                        : 0;

                    DmsResult getResult = dmsInstanceService.Get(path, getVer);
                    return CreateResponse(getResult.Data, true, getResult.MimeType, 200, headers);
                case "HEAD":
                    return CreateResponse(
                        Stream.Null,
                        false,
                        "text/xml; charset=\"utf-8\"",
                        204,
                        headers
                    );
                case "PROPFIND":
                    string propFindBody = PropFind(request, appId, path, requestText, ns, urlBase);
                    return CreateResponse(
                        EncodeText(propFindBody),
                        !string.IsNullOrEmpty(propFindBody),
                        "text/xml; charset=\"utf-8\"",
                        !string.IsNullOrEmpty(propFindBody) ? 207 : 404,
                        headers
                    );
                case "PROPPATCH":
                    string responseXmlElement = SerializeXml(
                        new XElement(
                        ns + "multistatus",
                        [
                            new XAttribute(XNamespace.Xmlns + "D", "DAV:"),
                            new XAttribute(XNamespace.Xmlns + "Z", "urn:schemas-microsoft-com:"),
                        ])
                    );

                    return CreateResponse(
                        EncodeText(responseXmlElement),
                        true,
                        "text/xml; charset=\"utf-8\"",
                        200,
                        headers
                    );
                case "POST":
                case "PUT":
                    request.Body.Position = 0;
                    await dmsInstanceService.SaveAsync(path, request.Body);
                    request.Body.Position = 0;
                    return CreateResponse(request.Body, true, request.ContentType, 201, headers);
                case "MKCOL":
                    request.Body.Position = 0;
                    await dmsInstanceService.SaveAsync(path, request.Body);
                    return CreateResponse(
                        Stream.Null,
                        false,
                        "text/xml; charset=\"utf-8\"",
                        204,
                        headers
                    );
                case "MOVE":
                    await dmsInstanceService.MoveAsync(
                        path,
                        ResolveDestinationPath(request, appId)
                    );
                    return CreateResponse(
                        Stream.Null,
                        false,
                        "text/xml; charset=\"utf-8\"",
                        204,
                        headers
                    );
                case "COPY":
                    await dmsInstanceService.CopyAsync(
                        path,
                        ResolveDestinationPath(request, appId)
                    );
                    return CreateResponse(
                        Stream.Null,
                        false,
                        "text/xml; charset=\"utf-8\"",
                        204,
                        headers
                    );
                case "DELETE":
                    await dmsInstanceService.DropAsync(
                        path,
                        int.TryParse(TryGetSingleValue(query, "version"), out int deleteVersion)
                            ? deleteVersion
                            : 0
                    );
                    return CreateResponse(
                        Stream.Null,
                        false,
                        "text/xml; charset=\"utf-8\"",
                        204,
                        headers
                    );
                case "LOCK":
                    return CreateResponse(
                        Stream.Null,
                        false,
                        "text/xml; charset=\"utf-8\"",
                        200,
                        headers
                    );
                case "UNLOCK":
                    return CreateResponse(
                        Stream.Null,
                        false,
                        "text/xml; charset=\"utf-8\"",
                        204,
                        headers
                    );
                default:
                    throw new InvalidOperationException(
                        $"Unsupported WebDAV method: {request.Method}"
                    );
            }
        }
        catch (System.Security.SecurityException)
        {
            headers.Add(
                new KeyValuePair<string, string>("WWW-Authenticate", "Basic realm=\"server\"")
            );
            return CreateResponse(Stream.Null, false, "text/xml; charset=\"utf-8\"", 204, headers);
        }
        catch (Exception ex)
        {
            return CreateResponse(
                EncodeText(ex.Message),
                true,
                "text/xml; charset=\"utf-8\"",
                200,
                headers
            );
        }
    }

    private string PropFind(
        DmsProcessingRequest request,
        int appId,
        LocalPath path,
        string requestText,
        XNamespace ns,
        string urlBase
    )
    {
        XDocument requestBody =
            requestText.Length > 0 && request.ContentType == "application/xml"
                ? XDocument.Parse(requestText)
                : new XDocument();

        IEnumerable<string> requestedProperties = requestBody
            .Descendants(ns + "prop")
            .DescendantNodes()
            .Select(node => ((XElement)node).Name.LocalName);

        return !path.IsToFile
            ? PropFindFolder(request, appId, path, ns, urlBase, requestedProperties)
            : PropFindFile(appId, path, ns, urlBase, requestedProperties);
    }

    private string PropFindFile(
        int appId,
        LocalPath path,
        XNamespace ns,
        string urlBase,
        IEnumerable<string> requestedProperties
    )
    {
        try
        {
            LocalFile file = fileService
                .GetAll()
                .FirstOrDefault(item =>
                    item.Folder.AppId == appId
                    && path.Length > 0
                    && item.Path.Equals(path.FullPath, StringComparison.CurrentCultureIgnoreCase)
                );

            XElement response = file?.ToWebDavResponse(urlBase, ns, requestedProperties);

            return SerializeXml(new XElement(
                ns + "multistatus",
                new object[]
                {
                    new XAttribute(XNamespace.Xmlns + "D", "DAV:"),
                    new XAttribute(XNamespace.Xmlns + "Z", "urn:schemas-microsoft-com:"),
                }.Union([response])
            ));
        }
        catch (Exception ex)
        {
            log.LogError(ex.Message + "\n" + ex.StackTrace);
            return string.Empty;
        }
    }

    private string PropFindFolder(
        DmsProcessingRequest request,
        int appId,
        LocalPath path,
        XNamespace ns,
        string urlBase,
        IEnumerable<string> requestedProperties
    )
    {
        LocalFolder folder =
            path.FullPath != string.Empty
                ? folderService
                    .GetAll()
                    .FirstOrDefault(item => item.AppId == appId && item.Path == path.FullPath)
                : new LocalFolder
                {
                    Name = "Root",
                    SubFolders =
                    [
                        .. folderService
                            .GetAll()
                            .Where(item => item.AppId == appId && item.ParentId == null),
                    ],
                    Files = [],
                    Path = string.Empty,
                    AppId = appId,
                };

        List<LocalFolder> folders = [];
        List<LocalFile> files = [];

        if (int.TryParse(GetHeaderValue(request, "Depth"), out int depth) && depth > 0)
        {
            folders =
            [
                .. folderService
                    .GetAll()
                    .Where(item =>
                        item.AppId == appId
                        && (
                            path.FullPath != string.Empty
                                ? item.Parent.Path.Equals(
                                    path.FullPath,
                                    StringComparison.CurrentCultureIgnoreCase
                                )
                                : item.ParentId == null
                        )
                    )
            ];

            files =
            [
                .. fileService
                    .GetAll()
                    .Where(item =>
                        item.Folder.AppId == appId
                        && path.Length > 0
                        && item.Folder.Path == path.FullPath
                    )
            ];
        }

        if (folder != null)
        {
            folders.Insert(0, folder);
        }

        IEnumerable<XElement> response = folders
            .Select(item => item.ToWebDavResponse(urlBase, ns, requestedProperties))
            .Union(files.Select(item => item.ToWebDavResponse(urlBase, ns, requestedProperties)));

        return SerializeXml(new XElement(
            ns + "multistatus",
            new object[]
            {
                new XAttribute(XNamespace.Xmlns + "D", "DAV:"),
                new XAttribute(XNamespace.Xmlns + "Z", "urn:schemas-microsoft-com:"),
            }.Union(response)
        ));
    }

    private static LocalPath ResolveDestinationPath(DmsProcessingRequest request, int appId)
    {
        string destination = WebUtility.UrlDecode(GetHeaderValue(request, "Destination"));
        string marker = $"Core/App({appId})/DAV/";
        int markerIndex = destination.IndexOf(marker, StringComparison.OrdinalIgnoreCase);

        if (markerIndex >= 0)
        {
            destination = destination[(markerIndex + marker.Length)..];
        }

        return new LocalPath(destination.TrimStart('/').TrimEnd('/'));
    }

    private static int ExtractAppId(string requestPath)
    {
        string marker = "Core/App(";
        int markerIndex = requestPath.IndexOf(marker, StringComparison.OrdinalIgnoreCase);

        if (markerIndex < 0)
            throw new FormatException($"Unable to resolve AppId from request path '{requestPath}'.");

        int start = markerIndex + marker.Length;
        int end = requestPath.IndexOf(')', start);

        if (end <= start || !int.TryParse(requestPath[start..end], out int appId))
            throw new FormatException($"Unable to resolve AppId from request path '{requestPath}'.");

        return appId;
    }

    private static string NormalizeRequestPath(string requestPath, int appId)
    {
        string marker = $"Core/App({appId})/DAV";
        int markerIndex = requestPath.IndexOf(marker, StringComparison.OrdinalIgnoreCase);

        return markerIndex >= 0
            ? requestPath[(markerIndex + marker.Length)..]
            : requestPath;
    }

    private static async ValueTask<string> ReadRequestBodyTextAsync(Stream body)
    {
        if (!body.CanRead)
        {
            return string.Empty;
        }

        if (body.CanSeek)
        {
            body.Position = 0;
        }

        using MemoryStream memoryStream = new();
        await body.CopyToAsync(memoryStream);

        if (body.CanSeek)
        {
            body.Position = 0;
        }

        return Encoding.UTF8.GetString(memoryStream.ToArray());
    }

    private static MemoryStream EncodeText(string content)
    {
        MemoryStream stream = new(Encoding.UTF8.GetBytes(content ?? string.Empty));
        stream.Position = 0;
        return stream;
    }

    private static string SerializeXml(XElement element) =>
        element.ToString(SaveOptions.DisableFormatting);

    private static DmsProcessingResponse CreateResponse(
        Stream body,
        bool hasBody,
        string contentType,
        int statusCode,
        List<KeyValuePair<string, string>> headers
    ) =>
        new()
        {
            Body = body,
            HasBody = hasBody,
            ContentType = contentType,
            StatusCode = statusCode,
            Headers = headers,
        };

    private static string TryGetSingleValue(Dictionary<string, string[]> query, string key) =>
        query.TryGetValue(key, out string[] values) ? values.FirstOrDefault() : null;

    private static string GetHeaderValue(DmsProcessingRequest request, string key) =>
        request.Headers.TryGetValue(key, out string[] values)
            ? values.FirstOrDefault() ?? string.Empty
            : string.Empty;

    private static Dictionary<string, string[]> ParseQuery(string queryString)
    {
        Dictionary<string, List<string>> query = [];
        string normalizedQuery = queryString?.TrimStart('?') ?? string.Empty;

        if (string.IsNullOrWhiteSpace(normalizedQuery))
        {
            return [];
        }

        foreach (string pair in normalizedQuery.Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            string[] parts = pair.Split('=', 2);
            string key = Uri.UnescapeDataString(parts[0]);
            string value = parts.Length > 1 ? Uri.UnescapeDataString(parts[1]) : string.Empty;

            if (!query.TryGetValue(key, out List<string> values))
            {
                values = [];
                query[key] = values;
            }

            values.Add(value);
        }

        return query.ToDictionary(
            entry => entry.Key,
            entry => entry.Value.ToArray(),
            StringComparer.OrdinalIgnoreCase
        );
    }
}










