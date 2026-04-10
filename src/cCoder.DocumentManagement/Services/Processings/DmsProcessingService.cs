using System.Security;
using cCoder.DocumentManagement.Models;
using cCoder.DocumentManagement.Services.Foundations;
using LocalPath = cCoder.DocumentManagement.Models.Path;
using DmsResult = cCoder.DocumentManagement.Models.DMSResult;
using MemoryStream = System.IO.MemoryStream;


namespace cCoder.DocumentManagement.Services.Processings;

internal class DmsProcessingService(
    IDmsInstanceService dmsInstanceService,
    ILogger<DmsProcessingService> log
) : IDmsProcessingService
{
    public async ValueTask<DmsProcessingResponse> ProcessAsync(DmsProcessingRequest request)
    {
        string path = request.RequestPath[
            (request.RequestPath.IndexOf("/dms/", StringComparison.CurrentCultureIgnoreCase) + 5)..
        ];
        Dictionary<string, string[]> query = ParseQuery(request.QueryString);

        try
        {
            switch (request.Method)
            {
                case "OPTIONS":
                    log.LogInformation($"DMS({request.App.Id}) OPTIONS {path}");
                    return await HandleOptionsRequestAsync(request);
                case "GET":
                    log.LogInformation($"DMS({request.App.Id}) Get {path}");
                    return await HandleGetRequestAsync(request);
                case "POST":
                    log.LogInformation($"DMS({request.App.Id}) POST {path}");
                    return await HandlePostRequestAsync(request);
                case "PUT":
                    log.LogInformation($"DMS({request.App.Id}) PUT {path}");
                    return await HandlePutRequestAsync(request);
                case "DELETE":
                    log.LogInformation($"DMS({request.App.Id}) DELETE {path}");
                    return await HandleDeleteRequestAsync(request);
                default:
                    throw new InvalidOperationException(
                        $"Unsupported DMS method: {request.Method}"
                    );
            }
        }
        catch (SecurityException ex)
        {
            log.LogError(
                $"An unhandled exception occurred whilst processing a DMS request to app on domain {request.App?.Domain ?? "Unknown"} ..."
            );
            log.LogError($"Request details - Path: {path}, Query: {request.QueryString}");
            log.LogError(ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            log.LogError(
                $"An unhandled exception occurred whilst processing a DMS request to app on domain {request.App?.Domain ?? "Unknown"} ..."
            );
            log.LogError($"Request details - Path: {path}, Query: {request.QueryString}");
            log.LogError(ex.Message);
            throw;
        }
    }

    private async ValueTask<DmsProcessingResponse> HandleDeleteRequestAsync(DmsProcessingRequest request)
    {
        string path = request.RequestPath[
            (request.RequestPath.IndexOf("/dms/", StringComparison.CurrentCultureIgnoreCase) + 5)..
        ];
        Dictionary<string, string[]> query = ParseQuery(request.QueryString);
        _ = int.TryParse(TryGetSingleValue(query, "version"), out int deleteVersion);
        await dmsInstanceService.DropAsync(new LocalPath(path), deleteVersion);
        return CreateResponse(Stream.Null, false, "application/json", 204);
    }

    private async ValueTask<DmsProcessingResponse> HandleOptionsRequestAsync(
        DmsProcessingRequest request
    ) => CreateResponse(Stream.Null, false, "application/json", 204);

    private async ValueTask<DmsProcessingResponse> HandleGetRequestAsync(DmsProcessingRequest request)
    {
        string path = request.RequestPath[
            (request.RequestPath.IndexOf("/dms/", StringComparison.CurrentCultureIgnoreCase) + 5)..
        ];
        Dictionary<string, string[]> query = ParseQuery(request.QueryString);
        bool download = query.ContainsKey("download");
        string search = TryGetSingleValue(query, "search") ?? string.Empty;
        int version = int.TryParse(TryGetSingleValue(query, "version"), out int parsedVersion)
            ? parsedVersion
            : 0;
        string[] downloadPaths =
            query.GetValueOrDefault("downloadPaths")?.FirstOrDefault()?.Split(",") ?? [];

        DmsResult result =
            downloadPaths.Length > 0
                ? dmsInstanceService.GetFilesZipped(
                    downloadPaths.Select(value => new LocalPath(value)).ToArray()
                )
                : dmsInstanceService.Get(new LocalPath(path), version, search);

        return result != null
            ? CreateResponse(
                result.Data,
                true,
                download ? "application/octet-stream" : result.MimeType,
                200
            )
            : CreateResponse(Stream.Null, false, "plain/text", 204);
    }

    private async ValueTask<DmsProcessingResponse> HandlePostRequestAsync(DmsProcessingRequest request)
    {
        string path = request.RequestPath[
            (request.RequestPath.IndexOf("/dms/", StringComparison.CurrentCultureIgnoreCase) + 5)..
        ];
        Dictionary<string, string[]> query = ParseQuery(request.QueryString);

        using MemoryStream memoryStream = new();
        await request.Body.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        if (query.ContainsKey("copyTo"))
        {
            string newPath = TryGetSingleValue(query, "copyTo");

            if (!string.IsNullOrWhiteSpace(newPath))
                await dmsInstanceService.CopyAsync(
                    new LocalPath(path.Split("?")[0]),
                    new LocalPath(newPath)
                );

            return CreateResponse(Stream.Null, false, "application/json", 204);
        }

        if (query.ContainsKey("moveTo"))
        {
            string newPath = TryGetSingleValue(query, "moveTo");

            if (!string.IsNullOrWhiteSpace(newPath))
                await dmsInstanceService.MoveAsync(
                    new LocalPath(path.Split("?")[0]),
                    new LocalPath(newPath)
                );

            return CreateResponse(Stream.Null, false, "application/json", 204);
        }

        return await SaveOrUnpackAsync(path, query, memoryStream);
    }

    private async ValueTask<DmsProcessingResponse> HandlePutRequestAsync(DmsProcessingRequest request)
    {
        string path = request.RequestPath[
            (request.RequestPath.IndexOf("/dms/", StringComparison.CurrentCultureIgnoreCase) + 5)..
        ];
        Dictionary<string, string[]> query = ParseQuery(request.QueryString);

        using MemoryStream memoryStream = new();
        await request.Body.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        if (query.ContainsKey("copyTo"))
        {
            string newPath = TryGetSingleValue(query, "copyTo");

            if (!string.IsNullOrWhiteSpace(newPath))
                await dmsInstanceService.CopyAsync(
                    new LocalPath(path.Split("?")[0]),
                    new LocalPath(newPath)
                );

            return CreateResponse(Stream.Null, false, "application/json", 204);
        }

        if (query.ContainsKey("moveTo"))
        {
            string newPath = TryGetSingleValue(query, "moveTo");

            if (!string.IsNullOrWhiteSpace(newPath))
                await dmsInstanceService.MoveAsync(
                    new LocalPath(path.Split("?")[0]),
                    new LocalPath(newPath)
                );

            return CreateResponse(Stream.Null, false, "application/json", 204);
        }

        return await SaveOrUnpackAsync(path, query, memoryStream);
    }

    private async ValueTask<DmsProcessingResponse> SaveOrUnpackAsync(
        string path,
        Dictionary<string, string[]> query,
        MemoryStream memoryStream
    )
    {
        LocalPath destinationPath = new(path);

        if (query.ContainsKey("unpack"))
        {
            if (destinationPath.IsToFile)
            {
                log.LogError(
                    $"User request to unpack an archive to a file path failed, The path is: {path}"
                );
                throw new InvalidOperationException("Cannot unpack an archive to a file path");
            }

            bool ignoreArchiveRoot = string.Equals(
                TryGetSingleValue(query, "ignoreArchiveRoot"),
                "true",
                StringComparison.OrdinalIgnoreCase
            );
            await dmsInstanceService.UnpackAsync(
                destinationPath,
                memoryStream,
                ignoreArchiveRoot
            );
        }
        else
        {
            await dmsInstanceService.SaveAsync(destinationPath, memoryStream);
        }

        return CreateResponse(Stream.Null, false, "application/json", 204);
    }

    private static DmsProcessingResponse CreateResponse(
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
        query.TryGetValue(key, out string[] values) ? values.FirstOrDefault() : null;

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








