// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.DocumentManagement.Services;
using cCoder.DocumentManagement.Services.Foundations;


namespace cCoder.DocumentManagement.Services.Processings;

internal partial class CurrentAppResolverProcessingService(
    ICurrentAppResolverService currentAppResolverService
) : ICurrentAppResolverProcessingService
{
    public App ResolveCurrentApp() =>
        TryCatch(operation: () =>
        {
            return ToResolvedApp(
                app: currentAppResolverService.ResolveCurrentApp());
        });

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