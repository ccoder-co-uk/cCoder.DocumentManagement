// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Services.Orchestrations;
using cCoder.DocumentManagement.Services.Processings;


namespace cCoder.DocumentManagement.Exposures.Middleware;

public class DMSMiddleware
{
    public DMSMiddleware(RequestDelegate next)
    {
        ArgumentNullException.ThrowIfNull(argument: next);
    }

    public async Task InvokeAsync(
        HttpContext context,
        IDmsHttpRequestOrchestrationService dmsHttpRequestOrchestrationService
    )
    {
        DmsProcessingResponse response =
            await dmsHttpRequestOrchestrationService.ProcessRequestAsync(context: context);
        await Respond(context: context, response: response);
    }

    private static async Task Respond(HttpContext context, DmsProcessingResponse response)
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
            context.Response.Headers.Append(key: "Content-Length", value: response.Body.Length.ToString());
            await response.Body.CopyToAsync(destination: context.Response.Body);
            response.Body.Close();
        }

        context.Response.Body.Close();
    }
}