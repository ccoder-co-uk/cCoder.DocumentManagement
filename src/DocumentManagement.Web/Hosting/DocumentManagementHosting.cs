// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using Apps.Shared;
using Apps.Shared.Models;
using cCoder.DocumentManagement;
using cCoder.Eventing;
using cCoder.Security;
using cCoder.Security.Data.EF;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.OData;


namespace DocumentManagement.Web.Hosting;

internal static class DocumentManagementHosting
{
    internal static void AddDocumentManagementApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string coreConnection = configuration.GetConnectionString(name: "Core")
            ?? throw new InvalidOperationException(message: "ConnectionStrings:Core is required.");

        string ssoConnection = configuration.GetConnectionString(name: "SSO")
            ?? throw new InvalidOperationException(message: "ConnectionStrings:SSO is required.");

        Config config = new();
        configuration.Bind(instance: config);
        services.AddSingleton(implementationInstance: config);
        services.AddEventing();

        services.AddSecurityApi(configAction: (securityServices, securityConfig) =>
        {
            securityConfig.AddMSSQLModelProvider(
                services: securityServices,
                connectionString: ssoConnection);

            securityConfig.UseAESHMMACPasswordEncryption(
                services: securityServices,
                decryptionKey: configuration.GetSection(key: "Settings")[key: "DecryptionKey"]);
        });

        cCoder.Data.IServiceCollectionExtensions.AddCoreData(
            services: services,
            connectionString: coreConnection);

        services.AddDocumentManagementWeb();
    }

    internal static void UseDocumentManagementApplication(this WebApplication app)
    {
        ILogger log = app.Services.GetRequiredService<ILogger<Program>>();

        app.UseHttpsRedirection();
        app.UseSession();
        app.UseStaticFiles();

        app.UseSwagger()
            .UseSwaggerUI(setupAction: options =>
            {
                options.SwaggerEndpoint(
                    url: "/swagger/DocumentManagement/swagger.json",
                    name: "DocumentManagement API");

                options.SwaggerEndpoint(
                    url: "/swagger/Core/swagger.json",
                    name: "Core API");

                options.SwaggerEndpoint(
                    url: "/swagger/v1/swagger.json",
                    name: "Core API");
            })
            .UseODataBatching()
            .UseODataRouteDebug();

        app.UseDomainApiShell();
        app.MapGet(pattern: "/Health", handler: () => Results.Text(content: "OK"));
        app.MapGet(pattern: "/", handler: () => Results.Redirect(url: "/tools/index.html"));
        app.StartDocumentManagementWeb(log: log);
        app.UseDomainDefaultCors();

        app.UseDomainExceptionHandling(
            errorHandler: context => HandleUnhandledException(
                context: context,
                log: log));
    }

    private static async Task HandleUnhandledException(
        HttpContext context,
        ILogger log)
    {
        Exception exception = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;

        context.Response.StatusCode =
            exception?.GetType() == typeof(SecurityException) ? 401 : 500;

        context.Response.ContentType = "application/json";

        if (exception is null)
        {
            return;
        }

        log.LogError(
            message: "{Message}\n{StackTrace}",
            exception.Message,
            exception.StackTrace);

        await context.Response.WriteAsync(
            text: "{ \"error\": \"" + exception.Message.Replace(oldValue: "\"", newValue: "\'") + "\" }");
    }
}