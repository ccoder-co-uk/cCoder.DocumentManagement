// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Reflection;
using cCoder.CodeAnalysis.Exposures;
using cCoder.DocumentManagement.Exposures;
using Xunit;

namespace cCoder.DocumentManagement.Tests;

public sealed partial class PackageRuntimeDependencyTests
{
    [Fact]
    public void DocumentManagementAssembly_WhenMarkersAreUsed_ReferencesRuntimeContractsOnly()
    {
        // Given
        Assembly documentManagementAssembly = typeof(IDmsInstanceFactory)
            .Assembly;

        string contractsAssemblyName = typeof(IUtilityBroker)
            .Assembly
            .GetName()
            .Name;

        string[] referencedAssemblyNames = documentManagementAssembly
            .GetReferencedAssemblies()
            .Select(selector: assemblyName => assemblyName.Name)
            .ToArray();

        // When
        bool referencesRuntimeContracts = referencedAssemblyNames
            .Contains(value: contractsAssemblyName);

        bool referencesAnalyzerRuntime = referencedAssemblyNames
            .Contains(value: "cCoder.CodeAnalysis");

        // Then
        Assert.True(
            condition: referencesRuntimeContracts,
            userMessage: "DocumentManagement must reference runtime marker contracts.");

        Assert.False(
            condition: referencesAnalyzerRuntime,
            userMessage: "DocumentManagement must not reference the analyzer runtime.");
    }
}