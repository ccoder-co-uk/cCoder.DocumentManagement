// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.DocumentManagement.Models;
using cCoder.DocumentManagement.Services.Foundations;
using LocalPath = cCoder.DocumentManagement.Models.Path;
using DmsResult = cCoder.DocumentManagement.Models.DMSResult;
using MemoryStream = System.IO.MemoryStream;


namespace cCoder.DocumentManagement.Services.Processings;

internal partial class DmsProcessingService(
    IDmsInstanceService dmsInstanceService,
    ILogger<DmsProcessingService> log
) : IDmsProcessingService
{
    public ValueTask<DmsProcessingResponse> ProcessDmsProcessingRequestAsync(DmsProcessingRequest request)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [request]);

            string path = request.RequestPath[
    (request.RequestPath.IndexOf(value: "/dms/", comparisonType: StringComparison.CurrentCultureIgnoreCase) + 5)..
];


            Dictionary<string, string[]> query = ParseQuery(queryString: request.QueryString);


            try
            {
                switch (request.Method)
                {
                    case "OPTIONS":
                        log.LogInformation(message: $"DMS({request.App.Id}) OPTIONS {path}");
                        return await HandleOptionsRequestDmsProcessingRequestAsync(request: request);
                    case "GET":
                        log.LogInformation(message: $"DMS({request.App.Id}) Get {path}");
                        return await HandleGetRequestDmsProcessingRequestAsync(request: request);
                    case "POST":
                        log.LogInformation(message: $"DMS({request.App.Id}) POST {path}");
                        return await HandlePostRequestDmsProcessingRequestAsync(request: request);
                    case "PUT":
                        log.LogInformation(message: $"DMS({request.App.Id}) PUT {path}");
                        return await HandlePutRequestDmsProcessingRequestAsync(request: request);
                    case "DELETE":
                        log.LogInformation(message: $"DMS({request.App.Id}) DELETE {path}");
                        return await HandleDeleteRequestDmsProcessingRequestAsync(request: request);
                    default:
                        throw new InvalidOperationException(
                            message: $"Unsupported DMS method: {request.Method}"
                        );
                }
            }
            catch (SecurityException ex)
            {
                log.LogError(
                    message: $"An unhandled exception occurred whilst processing a DMS request to app on domain {request.App?.Domain ?? "Unknown"} ..."
                );

                log.LogError(message: $"Request details - Path: {path}, Query: {request.QueryString}");
                log.LogError(message: ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                log.LogError(
                    message: $"An unhandled exception occurred whilst processing a DMS request to app on domain {request.App?.Domain ?? "Unknown"} ..."
                );

                log.LogError(message: $"Request details - Path: {path}, Query: {request.QueryString}");
                log.LogError(message: ex.Message);
                throw;
            }

        });

    private async ValueTask<DmsProcessingResponse> HandleDeleteRequestDmsProcessingRequestAsync(DmsProcessingRequest request)
    {
        string path = request.RequestPath[
            (request.RequestPath.IndexOf(value: "/dms/", comparisonType: StringComparison.CurrentCultureIgnoreCase) + 5)..
        ];

        Dictionary<string, string[]> query = ParseQuery(queryString: request.QueryString);
        _ = int.TryParse(s: TryGetSingleValue(query: query, key: "version"), result: out int deleteVersion);
        await dmsInstanceService.DropAsync(path: new LocalPath(path: path), version: deleteVersion);
        return CreateDmsProcessingResponse(body: Stream.Null, hasBody: false, contentType: "application/json", statusCode: 204);
    }

    private async ValueTask<DmsProcessingResponse> HandleOptionsRequestDmsProcessingRequestAsync(
        DmsProcessingRequest request
    ) =>
        CreateDmsProcessingResponse(body: Stream.Null, hasBody: false, contentType: "application/json", statusCode: 204);

    private async ValueTask<DmsProcessingResponse> HandleGetRequestDmsProcessingRequestAsync(DmsProcessingRequest request)
    {
        string path = request.RequestPath[
            (request.RequestPath.IndexOf(value: "/dms/", comparisonType: StringComparison.CurrentCultureIgnoreCase) + 5)..
        ];

        Dictionary<string, string[]> query = ParseQuery(queryString: request.QueryString);
        bool download = query.ContainsKey(key: "download");
        string search = TryGetSingleValue(query: query, key: "search") ?? string.Empty;

        int version = int.TryParse(s: TryGetSingleValue(query: query, key: "version"), result: out int parsedVersion)
            ? parsedVersion
            : 0;

        string[] downloadPaths =
            query.GetValueOrDefault(key: "downloadPaths")?.FirstOrDefault()?.Split(separator: ",") ?? [];

        DmsResult result =
            downloadPaths.Length > 0
                ? dmsInstanceService.GetFilesZipped(
                    paths: downloadPaths.Select(selector: value => new LocalPath(path: value))
            .ToArray()
                )
                : dmsInstanceService.Get(path: new LocalPath(path: path), version: version, search: search);

        return result != null
            ? CreateDmsProcessingResponse(
                body: result.Data,
                hasBody: true,
                contentType: download ? "application/octet-stream" : result.MimeType,
                statusCode: 200
            )
            : CreateDmsProcessingResponse(body: Stream.Null, hasBody: false, contentType: "plain/text", statusCode: 204);
    }

    private async ValueTask<DmsProcessingResponse> HandlePostRequestDmsProcessingRequestAsync(DmsProcessingRequest request)
    {
        string path = request.RequestPath[
            (request.RequestPath.IndexOf(value: "/dms/", comparisonType: StringComparison.CurrentCultureIgnoreCase) + 5)..
        ];

        Dictionary<string, string[]> query = ParseQuery(queryString: request.QueryString);

        using MemoryStream memoryStream = new();
        await request.Body.CopyToAsync(destination: memoryStream);
        memoryStream.Position = 0;

        if (query.ContainsKey(key: "copyTo"))
        {
            string newPath = TryGetSingleValue(query: query, key: "copyTo");

            if (!string.IsNullOrWhiteSpace(value: newPath))
            {
                await dmsInstanceService.CopyAsync(
                    oldPath: new LocalPath(path: path.Split(separator: "?")[0]),
                    newPath: new LocalPath(path: newPath)
                );
            }

            return CreateDmsProcessingResponse(body: Stream.Null, hasBody: false, contentType: "application/json", statusCode: 204);
        }

        if (query.ContainsKey(key: "moveTo"))
        {
            string newPath = TryGetSingleValue(query: query, key: "moveTo");

            if (!string.IsNullOrWhiteSpace(value: newPath))
            {
                await dmsInstanceService.MoveAsync(
                    oldPath: new LocalPath(path: path.Split(separator: "?")[0]),
                    newPath: new LocalPath(path: newPath)
                );
            }

            return CreateDmsProcessingResponse(body: Stream.Null, hasBody: false, contentType: "application/json", statusCode: 204);
        }

        return await SaveOrUnpackAsync(path: path, query: query, memoryStream: memoryStream);
    }

    private async ValueTask<DmsProcessingResponse> HandlePutRequestDmsProcessingRequestAsync(DmsProcessingRequest request)
    {
        string path = request.RequestPath[
            (request.RequestPath.IndexOf(value: "/dms/", comparisonType: StringComparison.CurrentCultureIgnoreCase) + 5)..
        ];

        Dictionary<string, string[]> query = ParseQuery(queryString: request.QueryString);

        using MemoryStream memoryStream = new();
        await request.Body.CopyToAsync(destination: memoryStream);
        memoryStream.Position = 0;

        if (query.ContainsKey(key: "copyTo"))
        {
            string newPath = TryGetSingleValue(query: query, key: "copyTo");

            if (!string.IsNullOrWhiteSpace(value: newPath))
            {
                await dmsInstanceService.CopyAsync(
                    oldPath: new LocalPath(path: path.Split(separator: "?")[0]),
                    newPath: new LocalPath(path: newPath)
                );
            }

            return CreateDmsProcessingResponse(body: Stream.Null, hasBody: false, contentType: "application/json", statusCode: 204);
        }

        if (query.ContainsKey(key: "moveTo"))
        {
            string newPath = TryGetSingleValue(query: query, key: "moveTo");

            if (!string.IsNullOrWhiteSpace(value: newPath))
            {
                await dmsInstanceService.MoveAsync(
                    oldPath: new LocalPath(path: path.Split(separator: "?")[0]),
                    newPath: new LocalPath(path: newPath)
                );
            }

            return CreateDmsProcessingResponse(body: Stream.Null, hasBody: false, contentType: "application/json", statusCode: 204);
        }

        return await SaveOrUnpackAsync(path: path, query: query, memoryStream: memoryStream);
    }

    private async ValueTask<DmsProcessingResponse> SaveOrUnpackAsync(
        string path,
        Dictionary<string, string[]> query,
        MemoryStream memoryStream
    )
    {
        LocalPath destinationPath = new(path: path);

        if (query.ContainsKey(key: "unpack"))
        {
            if (destinationPath.IsToFile)
            {
                log.LogError(
                    message: $"User request to unpack an archive to a file path failed, The path is: {path}"
                );

                throw new InvalidOperationException(message: "Cannot unpack an archive to a file path");
            }

            bool ignoreArchiveRoot = string.Equals(
                a: TryGetSingleValue(query: query, key: "ignoreArchiveRoot"),
                b: "true",
                comparisonType: StringComparison.OrdinalIgnoreCase
            );

            await dmsInstanceService.UnpackAsync(
                path: destinationPath,
                content: memoryStream,
                ignoreArchiveRoot: ignoreArchiveRoot
            );
        }
        else
        {
            await dmsInstanceService.SaveAsync(path: destinationPath, content: memoryStream);
        }

        return CreateDmsProcessingResponse(body: Stream.Null, hasBody: false, contentType: "application/json", statusCode: 204);
    }

    private static DmsProcessingResponse CreateDmsProcessingResponse(
        Stream body,
        bool hasBody,
        string contentType,
        int statusCode
    ) =>
        new()
        {
            Body = body,
            HasBody = hasBody,
            ContentType = contentType,
            StatusCode = statusCode,
        };

    private static string TryGetSingleValue(Dictionary<string, string[]> query, string key) =>
        query.TryGetValue(key: key, value: out string[] values) ? values.FirstOrDefault() : null;

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