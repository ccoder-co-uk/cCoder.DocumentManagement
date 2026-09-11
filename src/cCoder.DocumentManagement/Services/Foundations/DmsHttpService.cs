// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.DocumentManagement.Brokers;
using cCoder.DocumentManagement.Brokers.Storage;

namespace cCoder.DocumentManagement.Services.Foundations;

internal sealed partial class DmsHttpService(
    IAppBroker appBroker,
    IDmsHttpBroker dmsHttpBroker)
    : IDmsHttpService
{
    public DmsHttpSession BuildDmsHttpSession(DmsHttpSession dmsHttpSession) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [dmsHttpSession]);
            dmsHttpSession.App = ResolveApp(httpContext: dmsHttpSession.HttpContext);

            dmsHttpSession.Request = dmsHttpBroker.BuildRequest(
                httpContext: dmsHttpSession.HttpContext,
                app: dmsHttpSession.App);

            return dmsHttpSession;
        });

    public ValueTask<DmsHttpSession> WriteDmsHttpSessionAsync(
        DmsHttpSession dmsHttpSession) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [dmsHttpSession]);

            foreach (KeyValuePair<string, string> header in dmsHttpSession.Response.Headers)
            {
                if (!dmsHttpBroker.ContainsResponseHeader(
                    httpContext: dmsHttpSession.HttpContext,
                    key: header.Key))
                {
                    dmsHttpBroker.AppendResponseHeader(
                        httpContext: dmsHttpSession.HttpContext,
                        key: header.Key,
                        value: header.Value);
                }
            }

            dmsHttpBroker.SetResponseMetadata(
                httpContext: dmsHttpSession.HttpContext,
                dmsProcessingResponse: dmsHttpSession.Response);

            if (dmsHttpSession.Response.HasBody)
            {
                dmsHttpBroker.AppendResponseHeader(
                    httpContext: dmsHttpSession.HttpContext,
                    key: "Content-Length",
                    value: dmsHttpSession.Response.Body.Length.ToString());

                await dmsHttpBroker.CopyResponseBodyAsync(
                    httpContext: dmsHttpSession.HttpContext,
                    dmsProcessingResponse: dmsHttpSession.Response);

                dmsHttpBroker.CloseResponseBody(
                    dmsProcessingResponse: dmsHttpSession.Response);
            }

            return dmsHttpSession;
        });

    private App ResolveApp(HttpContext httpContext)
    {
        string requestPath = httpContext?.Request.Path.Value ?? string.Empty;

        if (
            requestPath.Contains(value: "/webdav", comparisonType: StringComparison.OrdinalIgnoreCase)
            && requestPath.Contains(value: "Core/App(", comparisonType: StringComparison.OrdinalIgnoreCase))
        {
            int start = requestPath.IndexOf(value: "Core/App(", comparisonType: StringComparison.OrdinalIgnoreCase) + 9;
            int end = requestPath.IndexOf(value: ')', startIndex: start);

            if (end > start && int.TryParse(s: requestPath[start..end], result: out int appId))
            {
                return ToResolvedApp(app: appBroker.SelectAppById(appId: appId))
                    ?? throw new InvalidOperationException(message: $"Unable to resolve app '{appId}'.");
            }
        }

        string host = httpContext?.Request.Host.Host ?? string.Empty;

        return ToResolvedApp(app: appBroker.SelectAppByDomain(domain: host))
            ?? throw new InvalidOperationException(message: $"Unable to resolve current app for host '{host}'.");
    }

    private static App ToResolvedApp(App app) =>
        app == null
            ? null
            : new App
            {
                Id = app.Id,
                DefaultCultureId = app.DefaultCultureId,
                TenantId = app.TenantId,
                Name = app.Name,
                Domain = app.Domain,
                DefaultTheme = app.DefaultTheme,
                ConfigJson = app.ConfigJson,
                Roles = [],
                Folders = [],
            };
}