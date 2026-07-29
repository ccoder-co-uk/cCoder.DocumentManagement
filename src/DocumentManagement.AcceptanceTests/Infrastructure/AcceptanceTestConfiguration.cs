// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.Data.SqlClient;
namespace Web.AcceptanceTests.Infrastructure;

internal sealed class AcceptanceTestConfiguration
{
    private AcceptanceTestConfiguration(
        string coreConnectionString,
        string securityConnectionString,
        string decryptionKey)
    {
        CoreConnectionString = coreConnectionString;
        SecurityConnectionString = securityConnectionString;
        DecryptionKey = decryptionKey;
    }

    internal string CoreConnectionString { get; }

    internal string SecurityConnectionString { get; }

    internal string DecryptionKey { get; }

    internal static AcceptanceTestConfiguration Load()
    {
        string suffix = $"-acceptance-{Guid.NewGuid():N}";

        return new AcceptanceTestConfiguration(
            coreConnectionString: AddDatabaseSuffix(
                connectionString: ReadRequiredValue(
                    variableName:
                        "DocumentManagement__ConnectionString"),
                suffix: suffix),
            securityConnectionString: AddDatabaseSuffix(
                connectionString: ReadRequiredValue(
                    variableName:
                        "Security__ConnectionString"),
                suffix: suffix),
            decryptionKey: ReadRequiredValue(
                variableName: "Security__DecryptionKey"));
    }

    private static string AddDatabaseSuffix(
        string connectionString,
        string suffix)
    {
        SqlConnectionStringBuilder builder =
            new(connectionString: connectionString)
            {
                Encrypt = true,
                TrustServerCertificate = true,
            };

        if (string.IsNullOrWhiteSpace(value: builder.InitialCatalog))
        {
            throw new InvalidOperationException(
                "Acceptance test connection strings must name a database.");
        }

        builder.InitialCatalog =
            $"{builder.InitialCatalog}{suffix}";

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