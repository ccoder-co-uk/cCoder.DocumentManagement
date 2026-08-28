// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.DocumentManagement.Models;
using cCoder.Eventing.Models;
using cCoder.Security.Models;
using Xunit;

namespace DocumentManagement.AcceptanceTests.Tests;

public sealed partial class AppConfigurationTests
{
    [Fact]
    public void ShouldExposeEveryRequiredDomainConfiguration()
    {
        // Given
        const string typeName =
            "DocumentManagement.Web.Models.AppConfiguration, DocumentManagement.Web";

        // When
        Type configurationType = Type.GetType(typeName: typeName);

        // Then
        Assert.NotNull(@object: configurationType);

        Assert.Equal(
            expected: typeof(CoreDataConfiguration),
            actual: configurationType.GetProperty(name: "CoreData")?.PropertyType);

        Assert.Equal(
            expected: typeof(DocumentManagementConfiguration),
            actual: configurationType.GetProperty(name: "DocumentManagement")?.PropertyType);

        Assert.Equal(
            expected: typeof(EventingConfiguration),
            actual: configurationType.GetProperty(name: "Eventing")?.PropertyType);

        Assert.Equal(
            expected: typeof(SecurityConfiguration),
            actual: configurationType.GetProperty(name: "Security")?.PropertyType);

        Assert.Equal(
            expected: typeof(SecurityDataConfiguration),
            actual: configurationType.GetProperty(name: "SecurityData")?.PropertyType);
    }
}