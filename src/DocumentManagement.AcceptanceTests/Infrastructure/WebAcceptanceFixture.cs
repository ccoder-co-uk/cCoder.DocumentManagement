// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using DocumentManagement.Web.Models;
using Web.AcceptanceTests.Models;
using Xunit;


namespace Web.AcceptanceTests.Infrastructure;

public sealed class WebAcceptanceFixture : IAsyncLifetime
{
    private AcceptanceDatabaseManager databaseManager;

    internal WebAcceptanceFactory Factory { get; private set; } = null!;

    public HttpClient Client { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        DocumentManagementWebConfiguration configuration =
            LoadTestConfiguration();

        AcceptanceSettings settings = new()
        {
            CoreConnectionString = AddDatabaseSuffix(
                connectionString:
                    configuration.DocumentManagement.ConnectionString),
            SsoConnectionString = AddDatabaseSuffix(
                connectionString:
                    configuration.Security.ConnectionString),
            DecryptionKey = configuration.Security.DecryptionKey
        };

        Factory = new WebAcceptanceFactory(settings: settings);
        databaseManager = new AcceptanceDatabaseManager(services: Factory.Services);
        await databaseManager.ResetDatabasesAsync();
        await SeedAsync();

        Client = Factory.CreateClient(options: new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri(uriString: "https://localhost"),
        });
    }

    public async Task DisposeAsync()
    {
        Client?.Dispose();

        if (databaseManager is not null)
        {
            await databaseManager.DropDatabasesAsync();
        }

        if (Factory is not null)
        {
            await Factory.DisposeAsync();
        }
    }

    private Task SeedAsync() =>
        new AcceptanceApplicationSeeder(services: Factory.Services).SeedAsync();

    private static string AddDatabaseSuffix(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(value: connectionString))
        {
            return string.Empty;
        }

        SqlConnectionStringBuilder builder = new(connectionString: connectionString)
        {
            Encrypt = true,
            TrustServerCertificate = true,
        };

        string databaseName = builder.InitialCatalog ?? string.Empty;

        if (string.IsNullOrWhiteSpace(value: databaseName))
        {
            return connectionString;
        }

        builder.InitialCatalog = $"{databaseName}-acceptance-{Guid.NewGuid():N}";
        return builder.ConnectionString;
    }

    private static DocumentManagementWebConfiguration LoadTestConfiguration()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(basePath: AppContext.BaseDirectory)
            .AddJsonFile(path: "appsettings.testing.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        DocumentManagementWebConfiguration result = new();
        configuration.Bind(instance: result);

        return result;
    }
}

[CollectionDefinition(Name)]
public sealed class WebAcceptanceCollection : ICollectionFixture<WebAcceptanceFixture>
{
    public const string Name = "Web acceptance";
}