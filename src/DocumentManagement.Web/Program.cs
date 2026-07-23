// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using Apps.Shared;
using Apps.Shared.Models;
using cCoder.DocumentManagement;
using cCoder.Security;
using cCoder.Security.Data.EF;
using cCoder.Eventing;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.OData;


namespace DocumentManagement.Web;

public class Program
{
    private static ILogger log = null!;

    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args: args);

        string coreConnection = builder.Configuration.GetConnectionString(name: "Core")
            ?? throw new InvalidOperationException(message: "ConnectionStrings:Core is required.");

        string ssoConnection = builder.Configuration.GetConnectionString(name: "SSO")
            ?? throw new InvalidOperationException(message: "ConnectionStrings:SSO is required.");

        Config config = new();
        builder.Configuration.Bind(instance: config);
        builder.Services.AddSingleton(implementationInstance: config);
        builder.Services.AddEventing();

        builder.Services.AddSecurityApi(configAction: (services, securityConfig) =>
        {
            securityConfig.AddMSSQLModelProvider(services, ssoConnection);
            securityConfig.UseAESHMMACPasswordEncryption(
                services,
                builder.Configuration.GetSection("Settings")["DecryptionKey"]);
        });

        cCoder.Data.IServiceCollectionExtensions.AddCoreData(
            services: builder.Services,
            connectionString: coreConnection);

        builder.Services.AddDocumentManagementWeb();

        WebApplication app = builder.Build();
        log = app.Services.GetRequiredService<ILogger<Program>>();

        app.UseHttpsRedirection();
        app.UseSession();
        app.UseStaticFiles();

        app.UseSwagger()
            .UseSwaggerUI(setupAction: options =>
            {
                options.SwaggerEndpoint("/swagger/DocumentManagement/swagger.json", "DocumentManagement API");
                options.SwaggerEndpoint("/swagger/Core/swagger.json", "Core API");
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Core API");
            })
            .UseODataBatching()
            .UseODataRouteDebug();

        app.UseDomainApiShell();
        app.MapGet(pattern: "/Health", handler: () => Results.Text("OK"));
        app.MapGet(pattern: "/", handler: () => Results.Redirect("/tools/index.html"));
        app.StartDocumentManagementWeb(log: log);
        app.UseDomainDefaultCors();
        app.UseDomainExceptionHandling(errorHandler: HandleUnhandledException);
        app.Run();
    }

    private static async Task HandleUnhandledException(HttpContext context)
    {
        Exception exception = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;

        context.Response.StatusCode =
            exception?.GetType() == typeof(SecurityException) ? 401 : 500;
        context.Response.ContentType = "application/json";

        if (exception is null)
        {
            return;
        }

        log.LogError(message: "{Message}\n{StackTrace}", exception.Message, exception.StackTrace);
        await context.Response.WriteAsync(
            text: "{ \"error\": \"" + exception.Message.Replace("\"", "\'") + "\" }");
    }
}