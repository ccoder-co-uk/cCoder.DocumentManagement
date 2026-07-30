// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Extensions.OData;
using cCoder.DocumentManagement.Models;
using cCoder.Data;
using cCoder.Eventing;
using Microsoft.OData.ModelBuilder;

namespace cCoder.DocumentManagement;

public static partial class IServiceCollectionExtensions
{
    internal static void RegisterDocumentManagementConfiguration(
        this IServiceCollection services,
        DocumentManagementConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(argument: configuration);
        services.AddSingleton(implementationInstance: configuration);

        if (!string.IsNullOrWhiteSpace(
            value: configuration.ConnectionString))
        {
            services.AddData(
                configuration: new cCoder.Data.Models.DataConfiguration
                {
                    ConnectionString = configuration.ConnectionString,
                    DebugInfo = configuration.DebugInfo,
                    LogSQL = configuration.LogSQL,
                });
        }

        services.AddEventProviders(eventProviders: configuration.EventProviders);
    }

    internal static void AddDocumentManagementApi(
        this IServiceCollection services,
        DocumentManagementConfiguration configuration,
        ODataConventionModelBuilder builder = null)
    {
        Action<ODataConventionModelBuilder> configureModel =
            static modelBuilder =>
                modelBuilder.ConfigureDocumentManagementApiModel();

        services.AddSingleton<Action<ODataConventionModelBuilder>>(implementationInstance: configureModel);

        if (builder is not null)
        {
            configureModel(obj: builder);
        }

        services.ConfigureDocumentManagementApi(
            configuration: configuration,
            configureModel: configureModel,
            includeDocumentation: builder is null);
    }
}