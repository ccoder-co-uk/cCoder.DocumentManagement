// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

using cCoder.CodeAnalysis.Exposures;
namespace cCoder.DocumentManagement.Exposures.Middleware;

public class DMSMiddleware(
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