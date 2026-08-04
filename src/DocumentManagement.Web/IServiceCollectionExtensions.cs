// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.DocumentManagement;
using cCoder.DocumentManagement.Extensions;
using cCoder.Data.Models;
using cCoder.Eventing.Models;
using cCoder.Security.Models;
using cCoder.Eventing;
using cCoder.Eventing.Http;
using cCoder.Eventing.Http.Models;
using cCoder.Security;
using DocumentManagement.Web.Models;

namespace DocumentManagement.Web;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddDocumentManagementWeb(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<DocumentManagementWebConfiguration> configure = null)
    {
        DocumentManagementWebConfiguration webConfiguration = new()
        {
            DocumentManagement = DocumentManagementConfigurationFactory.CreateDocumentManagementConfiguration(),
            Data = new DataConfiguration(),
            Security = new SecurityConfiguration(),
            Eventing = new EventingConfiguration()
        };
        configuration.Bind(instance: webConfiguration);
        configure?.Invoke(obj: webConfiguration);

        services.AddEventingWeb(configuration: webConfiguration.Eventing);
        services.AddHttpEventingHostedServices(
            configuration: new HttpEventingOptions
            {
                HubUrl = webConfiguration.Eventing.Http.HubUrl,
                MaxConcurrency =
                    webConfiguration.Eventing.Http.MaxConcurrency,
                JsonSerializerOptions =
                    new System.Text.Json.JsonSerializerOptions(
                        System.Text.Json.JsonSerializerDefaults.Web)
            });
        services.AddData(configuration: webConfiguration.Data);
        services.AddSecurityWeb(configuration: webConfiguration.Security);
        cCoder.DocumentManagement.IServiceCollectionExtensions
            .AddDocumentManagementWeb(
                services: services,
                configuration: webConfiguration.DocumentManagement);

        return services;
    }
}