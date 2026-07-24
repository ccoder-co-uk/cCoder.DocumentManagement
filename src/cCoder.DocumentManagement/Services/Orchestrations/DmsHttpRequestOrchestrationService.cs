// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Processings;


namespace cCoder.DocumentManagement.Services.Orchestrations;

internal partial class DmsHttpRequestOrchestrationService(
    ICurrentAppResolverProcessingService currentAppResolver,
    IDmsProcessingService dmsProcessingService,
    IWebDavProcessingService webDavProcessingService
) : IDmsHttpRequestOrchestrationService
{
    public ValueTask ProcessRequestAsync(HttpContext context)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [context]);
            App app = currentAppResolver.ResolveCurrentApp();

            DmsProcessingRequest request = BuildRequestApp(context: context, app: app);
            DmsProcessingResponse response;


            if (IsWebDavRequestDmsProcessingRequest(request: request))
            {
                response = await webDavProcessingService.ProcessDmsProcessingRequestAsync(request: request);
            }
            else
            {
                try
                {
                    DmsProcessingResponse processedResponse =
                        await dmsProcessingService.ProcessDmsProcessingRequestAsync(request: request);

                    response = AddDmsDefaultHeadersDmsProcessingResponse(
                        newDmsProcessingResponse: processedResponse);
                }
                catch (SecurityException)
                {
                    response = CreateDmsProcessingResponseForSecurity(host: request.Host);
                }
            }

            await WriteDmsProcessingResponseAsync(context: context, response: response);
        });

    private static async ValueTask WriteDmsProcessingResponseAsync(
        HttpContext context,
        DmsProcessingResponse response)
    {
        foreach (KeyValuePair<string, string> header in response.Headers)
        {
            if (!context.Response.Headers.ContainsKey(key: header.Key))
            {
                context.Response.Headers.Append(key: header.Key, value: header.Value);
            }
        }

        context.Response.ContentType = response.ContentType;
        context.Response.StatusCode = response.StatusCode;

        if (response.HasBody)
        {
            context.Response.Headers.Append(
                key: "Content-Length",
                value: response.Body.Length.ToString());

            await response.Body.CopyToAsync(destination: context.Response.Body);
            response.Body.Close();
        }
    }

    private static DmsProcessingRequest BuildRequestApp(HttpContext context, App app) =>
        new()
        {
            App = app,
            Method = context.Request.Method,
            RequestPath = context.Request.Path.Value ?? string.Empty,
            Host = context.Request.Host.Host,
            QueryString = context.Request.QueryString.Value ?? string.Empty,
            ContentType = context.Request.Headers.ContentType.ToString(),
            Body = context.Request.Body,
            Headers = context.Request.Headers.ToDictionary(
                keySelector: header => header.Key,
                elementSelector: header => header.Value.ToArray(),
                comparer: StringComparer.OrdinalIgnoreCase
            ),
        };

    private static bool IsWebDavRequestDmsProcessingRequest(DmsProcessingRequest request) =>
        request.RequestPath.Contains(value: "/webdav", comparisonType: StringComparison.OrdinalIgnoreCase);

    private static DmsProcessingResponse AddDmsDefaultHeadersDmsProcessingResponse(DmsProcessingResponse newDmsProcessingResponse)
    {
        List<KeyValuePair<string, string>> headers = [.. newDmsProcessingResponse.Headers];
        AddHeaderIfMissing(headers: headers, key: "Access-Control-Allow-Origin", value: "*");

        AddHeaderIfMissing(
            headers: headers,
            key: "Access-Control-Allow-Headers",
            value: "access-control-allow-origin,authorization,content-type,x-requested-with"
        );

        AddHeaderIfMissing(headers: headers, key: "Access-Control-Allow-Methods", value: "PUT,POST,GET,DELETE,OPTIONS");
        AddHeaderIfMissing(headers: headers, key: "Cache-Control", value: "max-age=2592000");

        return new DmsProcessingResponse
        {
            Body = newDmsProcessingResponse.Body,
            ContentType = newDmsProcessingResponse.ContentType,
            StatusCode = newDmsProcessingResponse.StatusCode,
            HasBody = newDmsProcessingResponse.HasBody,
            Headers = headers,
        };
    }

    private static DmsProcessingResponse CreateDmsProcessingResponseForSecurity(string host)
    {
        List<KeyValuePair<string, string>> headers =
        [
            new(key:"Access-Control-Allow-Origin", value:host),
            new(
                key:                "Access-Control-Allow-Headers",
                value:                "access-control-allow-origin,authorization,content-type,x-requested-with"
            ),
            new(key:"Access-Control-Allow-Methods", value:"PUT,POST,GET,DELETE,OPTIONS"),
            new(key:"Cache-Control", value:"max-age=2592000"),
        ];

        return new DmsProcessingResponse
        {
            ContentType = "application/json",
            StatusCode = 204,
            HasBody = false,
            Headers = headers,
        };
    }

    private static void AddHeaderIfMissing(
        List<KeyValuePair<string, string>> headers,
        string key,
        string value
    )
    {
        if (
            !headers.Any(predicate: header =>
                string.Equals(a: header.Key, b: key, comparisonType: StringComparison.OrdinalIgnoreCase)
            )
        )
        {
            headers.Add(item: new KeyValuePair<string, string>(key: key, value: value));
        }
    }
}
