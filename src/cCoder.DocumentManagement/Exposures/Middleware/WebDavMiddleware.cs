// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Services.Orchestrations;
namespace cCoder.DocumentManagement.Exposures.Middleware;

public class WebDavMiddleware
{
    public WebDavMiddleware(RequestDelegate next)
    {
        ArgumentNullException.ThrowIfNull(argument: next);
    }

    public async Task InvokeAsync(
        HttpContext context,
        IDmsHttpRequestOrchestrationService dmsHttpRequestOrchestrationService
    )
    {
        await dmsHttpRequestOrchestrationService.ProcessRequestAsync(context: context);
    }
}