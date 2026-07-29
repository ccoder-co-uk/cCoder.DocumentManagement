// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
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
        string suffix = $"-acceptance-{Guid.NewGuid():N}";

        AcceptanceSettings settings = new()
        {
            CoreConnectionString = AddDatabaseSuffix(
                connectionString: ReadRequiredValue(
                    variableName:
                        "DocumentManagement__ConnectionString"),
                suffix: suffix),
            SsoConnectionString = AddDatabaseSuffix(
                connectionString: ReadRequiredValue(
                    variableName: "Security__ConnectionString"),
                suffix: suffix),
            DecryptionKey = ReadRequiredValue(
                variableName: "Security__DecryptionKey"),
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

    private static string AddDatabaseSuffix(
        string connectionString,
        string suffix)
    {
        SqlConnectionStringBuilder builder = new(connectionString: connectionString)
        {
            Encrypt = true,
            TrustServerCertificate = true,
        };

        string databaseName = builder.InitialCatalog ?? string.Empty;

        if (string.IsNullOrWhiteSpace(value: databaseName))
        {
            throw new InvalidOperationException(
                "Acceptance test connection strings must name a database.");
        }

        builder.InitialCatalog = $"{databaseName}{suffix}";
        return builder.ConnectionString;
    }

    private static string ReadRequiredValue(string variableName)
    {
        string value =
            Environment.GetEnvironmentVariable(variable: variableName)
            ?? Environment.GetEnvironmentVariable(
                variable: variableName,
                target: EnvironmentVariableTarget.User)
            ?? Environment.GetEnvironmentVariable(
                variable: variableName,
                target: EnvironmentVariableTarget.Machine);

        if (!string.IsNullOrWhiteSpace(value: value))
        {
            return value;
        }

        throw new InvalidOperationException(
            $"Required configuration environment variable '{variableName}' was not found.");
    }
}

[CollectionDefinition(Name)]
public sealed class WebAcceptanceCollection : ICollectionFixture<WebAcceptanceFixture>
{
    public const string Name = "Web acceptance";
}