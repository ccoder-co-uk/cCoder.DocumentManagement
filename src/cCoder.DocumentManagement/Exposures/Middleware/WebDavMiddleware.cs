// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.CodeAnalysis.Exposures;
using cCoder.DocumentManagement.Services.Orchestrations;
namespace cCoder.DocumentManagement.Exposures.Middleware;

public class WebDavMiddleware(
    IDmsHttpRequestManager dmsHttpRequestManager)
    : IMiddleware, ICompositionExposure
{
    public async Task InvokeAsync(
        HttpContext context,
        RequestDelegate next
    )
    {
        await dmsHttpRequestManager.ProcessRequestAsync(context: context);
    }
}