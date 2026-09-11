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
    IDmsHttpProcessingService dmsHttpProcessingService,
    IDmsInstanceProcessingService dmsProcessingService,
    IWebDavProcessingService webDavProcessingService
) : IDmsHttpRequestOrchestrationService
{
    public ValueTask ProcessRequestAsync(HttpContext context)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [context]);

            DmsProcessingRequest request = dmsHttpProcessingService.BuildDmsHttpSession(
                dmsHttpSession: new DmsHttpSession
                {
                    HttpContext = context,
                }).Request;

            DmsProcessingResponse response;


            if (IsWebDavRequestDmsProcessingRequest(request: request))
            {
                DmsProcessingSession session =
                    await webDavProcessingService.ProcessDmsProcessingSessionAsync(
                        dmsProcessingSession: new DmsProcessingSession
                        {
                            Request = request
                        });

                response = session.Response;
            }
            else
            {
                try
                {
                    DmsProcessingSession session =
                        await dmsProcessingService.ProcessDmsProcessingSessionAsync(
                            dmsProcessingSession: new DmsProcessingSession
                            {
                                Request = request
                            });

                    response = AddDmsDefaultHeadersDmsProcessingResponse(
                        newDmsProcessingResponse: session.Response);
                }
                catch (SecurityException)
                {
                    response = CreateDmsProcessingResponseForSecurity(host: request.Host);
                }
            }

            await dmsHttpProcessingService.WriteDmsHttpSessionAsync(
                dmsHttpSession: new DmsHttpSession
                {
                    HttpContext = context,
                    Response = response,
                });
        });

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