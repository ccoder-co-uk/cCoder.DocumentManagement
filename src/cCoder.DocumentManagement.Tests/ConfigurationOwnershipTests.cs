// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.DocumentManagement.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace cCoder.DocumentManagement.Tests;

public sealed partial class ConfigurationOwnershipTests
{
    [Fact]
    public void DocumentManagementConfiguration_ShouldNotOwnPersistenceConfiguration()
    {
        // Given
        Type configurationType = typeof(DocumentManagementConfiguration);

        // When
        string[] propertyNames = configurationType
            .GetProperties()
            .Select(selector: property => property.Name)
            .ToArray();

        // Then
        propertyNames.Should()
            .NotContain(unexpected: [
                "ConnectionString",
                "DebugInfo",
                "LogSQL"]);
    }

    [Fact]
    public void AddDocumentManagementWeb_ShouldNotRegisterCoreDataServices()
    {
        // Given
        IServiceCollection services = new ServiceCollection();
        DocumentManagementConfiguration configuration = new();

        typeof(DocumentManagementConfiguration)
            .GetProperty(name: "ConnectionString")
            ?.SetValue(obj: configuration, value: "Server=(local);");

        // When
        services.AddDocumentManagementWeb(configuration: configuration);

        // Then
        services.Should()
            .NotContain(predicate: descriptor =>
                descriptor.ServiceType == typeof(CoreDataContext));
    }
}