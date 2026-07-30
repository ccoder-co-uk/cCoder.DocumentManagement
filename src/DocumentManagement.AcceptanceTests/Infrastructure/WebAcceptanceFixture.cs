// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Mvc.Testing;
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
        AcceptanceTestConfiguration configuration =
            AcceptanceTestConfiguration.Load();

        AcceptanceSettings settings = new()
        {
            CoreConnectionString =
                configuration.CoreConnectionString,
            SsoConnectionString =
                configuration.SecurityConnectionString,
            DecryptionKey = configuration.DecryptionKey,
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

}