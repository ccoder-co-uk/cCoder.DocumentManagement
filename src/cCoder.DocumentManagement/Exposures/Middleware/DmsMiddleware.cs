using cCoder.DocumentManagement.Services.Orchestrations;
using cCoder.DocumentManagement.Services.Processings;


namespace cCoder.DocumentManagement.Exposures.Middleware;

public class DMSMiddleware
{
    public DMSMiddleware(RequestDelegate next)
    {
        ArgumentNullException.ThrowIfNull(next);
    }

    public async Task InvokeAsync(
        HttpContext context,
        IDmsHttpRequestOrchestrationService dmsHttpRequestOrchestrationService
    )
    {
        DmsProcessingResponse response =
            await dmsHttpRequestOrchestrationService.ProcessRequestAsync(context);
        await Respond(context, response);
    }

    private static async Task Respond(HttpContext context, DmsProcessingResponse response)
    {
        foreach (KeyValuePair<string, string> header in response.Headers)
        {
            if (!context.Response.Headers.ContainsKey(header.Key))
                context.Response.Headers.Append(header.Key, header.Value);
        }

        context.Response.ContentType = response.ContentType;
        context.Response.StatusCode = response.StatusCode;

        if (response.HasBody)
        {
            context.Response.Headers.Append("Content-Length", response.Body.Length.ToString());
            await response.Body.CopyToAsync(context.Response.Body);
            response.Body.Close();
        }

        context.Response.Body.Close();
    }
}




