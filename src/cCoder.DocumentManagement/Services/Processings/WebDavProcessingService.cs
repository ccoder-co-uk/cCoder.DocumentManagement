// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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

internal partial class WebDavProcessingService(
    IFileService fileService,
    IFolderService folderService,
    IDmsInstanceService dmsInstanceService,
    Config config,
    ILogger<WebDavProcessingService> log
) : IWebDavProcessingService
{
    public ValueTask<DmsProcessingResponse> ProcessDmsProcessingRequestAsync(DmsProcessingRequest request)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [request]);
            int appId = ExtractAppId(requestPath: request.RequestPath);


            LocalPath path = new(
                path: WebUtility
                    .UrlDecode(encodedValue: NormalizeRequestPath(requestPath: request.RequestPath, appId: appId))
                    .TrimStart(trimChar: '/')
                    .TrimEnd(trimChar: '/')
            );


            string requestText = await ReadRequestBodyTextAsync(body: request.Body);


            log.LogDebug(message: $"HTTP {request.Method.ToUpperInvariant()} - {path} \n {requestText}");


            XNamespace ns = "DAV:";

            App app = request.App;

            Dictionary<string, string[]> query = ParseQuery(queryString: request.QueryString);


            string sslPort = config.Settings.TryGetValue(key: "sslPort", value: out string configuredSslPort)
                ? configuredSslPort
                : "443";


            string urlBase = $"https://{app.Domain}:{sslPort}/Api/";


            List<KeyValuePair<string, string>> headers =
            [
                new KeyValuePair<string, string>(key:"Host", value:urlBase + "DAV/"),
            ];


            if (!request.Headers.ContainsKey(key: "Authorization"))
            {
                headers.Add(
                    item: new KeyValuePair<string, string>(key: "WWW-Authenticate", value: "Basic realm=\"server\"")
                );

                headers.Add(item: new KeyValuePair<string, string>(key: "Connection", value: "close"));

                return CreateResponse(
                    body: EncodeText(content: string.Empty),
                    hasBody: true,
                    contentType: "text/xml; charset=\"utf-8\"",
                    statusCode: 401,
                    headers: headers
                );
            }


            try
            {
                switch (request.Method.ToUpperInvariant())
                {
                    case "OPTIONS":
                        headers.AddRange(collection: [
                            new KeyValuePair<string, string>(
                            key:                            "Access-Control-Allow-Origin",
                            value:                            request.Host
                        ),
                        new KeyValuePair<string, string>(
                            key:                            "Allow",
                            value:                            "OPTIONS, GET, HEAD, POST, PUT, DELETE, TRACE, COPY, MOVE, MKCOL, PROPFIND, PROPPATCH, LOCK, UNLOCK, ORDERPATCH"
                        ),
                        new KeyValuePair<string, string>(
                            key:                            "Public",
                            value:                            "OPTIONS, GET, HEAD, POST, PUT, DELETE, TRACE, COPY, MOVE, MKCOL, PROPFIND, PROPPATCH, LOCK, UNLOCK, ORDERPATCH"
                        ),
                        new KeyValuePair<string, string>(
                            key:                            "Date",
                            value:                            DateTimeOffset.Now.ToString(format:"s") + "Z"
                        ),
                        new KeyValuePair<string, string>(key:"DAV", value:"1, 2"),
                        new KeyValuePair<string, string>(key:"MS-Author-Via", value:"DAV"),
                        ]);

                        return CreateResponse(
                            body: Stream.Null,
                            hasBody: false,
                            contentType: "text/xml; charset=\"utf-8\"",
                            statusCode: 204,
                            headers: headers
                        );
                    case "GET":
                        int getVer = int.TryParse(
                            s: TryGetSingleValue(query: query, key: "version"),
                            result: out int parsedVersion
                        )
                            ? parsedVersion
                            : 0;

                        DmsResult getResult = dmsInstanceService.Get(path: path, version: getVer);
                        return CreateResponse(body: getResult.Data, hasBody: true, contentType: getResult.MimeType, statusCode: 200, headers: headers);
                    case "HEAD":
                        return CreateResponse(
                            body: Stream.Null,
                            hasBody: false,
                            contentType: "text/xml; charset=\"utf-8\"",
                            statusCode: 204,
                            headers: headers
                        );
                    case "PROPFIND":
                        string propFindBody = PropFindDmsProcessingRequest(request: request, appId: appId, path: path, requestText: requestText, ns: ns, urlBase: urlBase);
                        return CreateResponse(
                            body: EncodeText(content: propFindBody),
                            hasBody: !string.IsNullOrEmpty(value: propFindBody),
                            contentType: "text/xml; charset=\"utf-8\"",
                            statusCode: !string.IsNullOrEmpty(value: propFindBody) ? 207 : 404,
                            headers: headers
                        );
                    case "PROPPATCH":
                        string responseXmlElement = SerializeXml(
                            element: new XElement(
                            name: ns + "multistatus",
                            content: [
                                new XAttribute(name:XNamespace.Xmlns + "D", value:"DAV:"),
                            new XAttribute(name:XNamespace.Xmlns + "Z", value:"urn:schemas-microsoft-com:"),
                            ])
                        );

                        return CreateResponse(
                            body: EncodeText(content: responseXmlElement),
                            hasBody: true,
                            contentType: "text/xml; charset=\"utf-8\"",
                            statusCode: 200,
                            headers: headers
                        );
                    case "POST":
                    case "PUT":
                        request.Body.Position = 0;
                        await dmsInstanceService.SaveAsync(path: path, content: request.Body);
                        request.Body.Position = 0;
                        return CreateResponse(body: request.Body, hasBody: true, contentType: request.ContentType, statusCode: 201, headers: headers);
                    case "MKCOL":
                        request.Body.Position = 0;
                        await dmsInstanceService.SaveAsync(path: path, content: request.Body);
                        return CreateResponse(
                            body: Stream.Null,
                            hasBody: false,
                            contentType: "text/xml; charset=\"utf-8\"",
                            statusCode: 204,
                            headers: headers
                        );
                    case "MOVE":
                        await dmsInstanceService.MoveAsync(
                            oldPath: path,
                            newPath: ResolveDestinationPathDmsProcessingRequest(request: request, appId: appId)
                        );
                        return CreateResponse(
                            body: Stream.Null,
                            hasBody: false,
                            contentType: "text/xml; charset=\"utf-8\"",
                            statusCode: 204,
                            headers: headers
                        );
                    case "COPY":
                        await dmsInstanceService.CopyAsync(
                            oldPath: path,
                            newPath: ResolveDestinationPathDmsProcessingRequest(request: request, appId: appId)
                        );
                        return CreateResponse(
                            body: Stream.Null,
                            hasBody: false,
                            contentType: "text/xml; charset=\"utf-8\"",
                            statusCode: 204,
                            headers: headers
                        );
                    case "DELETE":
                        await dmsInstanceService.DropAsync(
                            path: path,
                            version: int.TryParse(s: TryGetSingleValue(query: query, key: "version"), result: out int deleteVersion)
                                ? deleteVersion
                                : 0
                        );
                        return CreateResponse(
                            body: Stream.Null,
                            hasBody: false,
                            contentType: "text/xml; charset=\"utf-8\"",
                            statusCode: 204,
                            headers: headers
                        );
                    case "LOCK":
                        return CreateResponse(
                            body: Stream.Null,
                            hasBody: false,
                            contentType: "text/xml; charset=\"utf-8\"",
                            statusCode: 200,
                            headers: headers
                        );
                    case "UNLOCK":
                        return CreateResponse(
                            body: Stream.Null,
                            hasBody: false,
                            contentType: "text/xml; charset=\"utf-8\"",
                            statusCode: 204,
                            headers: headers
                        );
                    default:
                        throw new InvalidOperationException(
                            message: $"Unsupported WebDAV method: {request.Method}"
                        );
                }
            }
            catch (System.Security.SecurityException)
            {
                headers.Add(
                    item: new KeyValuePair<string, string>(key: "WWW-Authenticate", value: "Basic realm=\"server\"")
                );

                return CreateResponse(body: Stream.Null, hasBody: false, contentType: "text/xml; charset=\"utf-8\"", statusCode: 204, headers: headers);
            }
            catch (Exception ex)
            {
                return CreateResponse(
                    body: EncodeText(content: ex.Message),
                    hasBody: true,
                    contentType: "text/xml; charset=\"utf-8\"",
                    statusCode: 200,
                    headers: headers
                );
            }

        });

    private string PropFindDmsProcessingRequest(
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
                ? XDocument.Parse(text: requestText)
                : new XDocument();

        IEnumerable<string> requestedProperties = requestBody
            .Descendants(name: ns + "prop")
            .DescendantNodes()
            .Select(selector: node => ((XElement)node).Name.LocalName);

        return !path.IsToFile
            ? PropFindFolderDmsProcessingRequest(request: request, appId: appId, path: path, ns: ns, urlBase: urlBase, requestedProperties: requestedProperties)
            : PropFindFile(appId: appId, path: path, ns: ns, urlBase: urlBase, requestedProperties: requestedProperties);
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
                .FirstOrDefault(predicate: item =>
                    item.Folder.AppId == appId
                    && path.Length > 0
                    && item.Path.Equals(value: path.FullPath, comparisonType: StringComparison.CurrentCultureIgnoreCase)
                );

            XElement response = file?.ToWebDavResponse(urlBase: urlBase, ns: ns, requestedProperties: requestedProperties);

            return SerializeXml(element: new XElement(
                name: ns + "multistatus",
                content: new object[]
                {
                    new XAttribute(name:XNamespace.Xmlns + "D", value:"DAV:"),
                    new XAttribute(name:XNamespace.Xmlns + "Z", value:"urn:schemas-microsoft-com:"),
                }.Union(second: [response])
            ));
        }
        catch (Exception ex)
        {
            log.LogError(message: ex.Message + "\n" + ex.StackTrace);
            return string.Empty;
        }
    }

    private string PropFindFolderDmsProcessingRequest(
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
                    .FirstOrDefault(predicate: item => item.AppId == appId && item.Path == path.FullPath)
                : new LocalFolder
                {
                    Name = "Root",
                    SubFolders =
                    [
                        .. folderService
                            .GetAll()
                            .Where(predicate:item => item.AppId == appId && item.ParentId == null),
                    ],
                    Files = [],
                    Path = string.Empty,
                    AppId = appId,
                };

        List<LocalFolder> folders = [];
        List<LocalFile> files = [];

        if (int.TryParse(s: GetHeaderValueDmsProcessingRequest(request: request, key: "Depth"), result: out int depth) && depth > 0)
        {
            folders =
            [
                .. folderService
                    .GetAll()
                    .Where(predicate:item =>
                        item.AppId == appId
                        && (
                            path.FullPath != string.Empty
                                ? item.Parent.Path.Equals(
                                    value:                                    path.FullPath,
                                    comparisonType:                                    StringComparison.CurrentCultureIgnoreCase
                                )
                                : item.ParentId == null
                        )
                    )
            ];

            files =
            [
                .. fileService
                    .GetAll()
                    .Where(predicate:item =>
                        item.Folder.AppId == appId
                        && path.Length > 0
                        && item.Folder.Path == path.FullPath
                    )
            ];
        }

        if (folder != null)
        {
            folders.Insert(index: 0, item: folder);
        }

        IEnumerable<XElement> response = folders
            .Select(selector: item => item.ToWebDavResponse(urlBase: urlBase, ns: ns, requestedProperties: requestedProperties))
            .Union(second: files.Select(selector: item => item.ToWebDavResponse(urlBase: urlBase, ns: ns, requestedProperties: requestedProperties)));

        return SerializeXml(element: new XElement(
            name: ns + "multistatus",
            content: new object[]
            {
                new XAttribute(name:XNamespace.Xmlns + "D", value:"DAV:"),
                new XAttribute(name:XNamespace.Xmlns + "Z", value:"urn:schemas-microsoft-com:"),
            }.Union(second: response)
        ));
    }

    private static LocalPath ResolveDestinationPathDmsProcessingRequest(DmsProcessingRequest request, int appId)
    {
        string destination = WebUtility.UrlDecode(encodedValue: GetHeaderValueDmsProcessingRequest(request: request, key: "Destination"));
        string marker = $"Core/App({appId})/DAV/";
        int markerIndex = destination.IndexOf(value: marker, comparisonType: StringComparison.OrdinalIgnoreCase);

        if (markerIndex >= 0)
        {
            destination = destination[(markerIndex + marker.Length)..];
        }

        return new LocalPath(path: destination.TrimStart(trimChar: '/')
            .TrimEnd(trimChar: '/'));
    }

    private static int ExtractAppId(string requestPath)
    {
        string marker = "Core/App(";
        int markerIndex = requestPath.IndexOf(value: marker, comparisonType: StringComparison.OrdinalIgnoreCase);

        if (markerIndex < 0)
        {
            throw new FormatException(message: $"Unable to resolve AppId from request path '{requestPath}'.");
        }

        int start = markerIndex + marker.Length;
        int end = requestPath.IndexOf(value: ')', startIndex: start);

        if (end <= start || !int.TryParse(s: requestPath[start..end], result: out int appId))
        {
            throw new FormatException(message: $"Unable to resolve AppId from request path '{requestPath}'.");
        }

        return appId;
    }

    private static string NormalizeRequestPath(string requestPath, int appId)
    {
        string marker = $"Core/App({appId})/DAV";
        int markerIndex = requestPath.IndexOf(value: marker, comparisonType: StringComparison.OrdinalIgnoreCase);

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
        await body.CopyToAsync(destination: memoryStream);

        if (body.CanSeek)
        {
            body.Position = 0;
        }

        return Encoding.UTF8.GetString(bytes: memoryStream.ToArray());
    }

    private static MemoryStream EncodeText(string content)
    {
        MemoryStream stream = new(buffer: Encoding.UTF8.GetBytes(s: content ?? string.Empty));
        stream.Position = 0;
        return stream;
    }

    private static string SerializeXml(XElement element) =>
        element.ToString(options: SaveOptions.DisableFormatting);

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
        query.TryGetValue(key: key, value: out string[] values) ? values.FirstOrDefault() : null;

    private static string GetHeaderValueDmsProcessingRequest(DmsProcessingRequest request, string key) =>
        request.Headers.TryGetValue(key: key, value: out string[] values)
            ? values.FirstOrDefault() ?? string.Empty
            : string.Empty;

    private static Dictionary<string, string[]> ParseQuery(string queryString)
    {
        Dictionary<string, List<string>> query = [];
        string normalizedQuery = queryString?.TrimStart(trimChar: '?') ?? string.Empty;

        if (string.IsNullOrWhiteSpace(value: normalizedQuery))
        {
            return [];
        }

        foreach (string pair in normalizedQuery.Split(separator: '&', options: StringSplitOptions.RemoveEmptyEntries))
        {
            string[] parts = pair.Split(separator: '=', count: 2);
            string key = Uri.UnescapeDataString(stringToUnescape: parts[0]);
            string value = parts.Length > 1 ? Uri.UnescapeDataString(stringToUnescape: parts[1]) : string.Empty;

            if (!query.TryGetValue(key: key, value: out List<string> values))
            {
                values = [];
                query[key: key] = values;
            }

            values.Add(item: value);
        }

        return query.ToDictionary(
            keySelector: entry => entry.Key,
            elementSelector: entry => entry.Value.ToArray(),
            comparer: StringComparer.OrdinalIgnoreCase
        );
    }
}