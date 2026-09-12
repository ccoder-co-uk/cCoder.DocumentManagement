// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Reflection;
using cCoder.DocumentManagement.Exposures.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace cCoder.DocumentManagement.Tests.Exposures;

public partial class ControllerSurfaceContractTests
{
    [Fact]
    public void Controllers_WhenNotMetadataOnly_DoNotExposeGetMetadata()
    {
        // Given
        IEnumerable<MethodInfo> controllerMethods = GetControllerMethods();

        // When
        IEnumerable<MethodInfo> metadataMethods = controllerMethods
            .Where(predicate: method => method.Name == "GetMetadata");

        // Then
        metadataMethods.Should()
            .BeEmpty();
    }

    [Fact]
    public void Controllers_DoNotExposePatchOrMerge()
    {
        // Given
        IEnumerable<MethodInfo> controllerMethods = GetControllerMethods();

        // When
        IEnumerable<MethodInfo> patchMethods = controllerMethods
            .Where(predicate: ExposesPatchOrMerge);

        // Then
        patchMethods.Should()
            .BeEmpty();
    }

    private static IEnumerable<MethodInfo> GetControllerMethods() =>
        typeof(FileController).Assembly.GetTypes()
            .Where(predicate: type => type.IsSubclassOf(c: typeof(ControllerBase)))
            .Where(predicate: type => !type.Name.EndsWith(
                value: "MetadataController",
                comparisonType: StringComparison.Ordinal))
            .SelectMany(selector: type => type.GetMethods(
                bindingAttr: BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly));

    private static bool ExposesPatchOrMerge(MethodInfo method) =>
        method.GetCustomAttributes(inherit: true)
            .OfType<AcceptVerbsAttribute>()
            .SelectMany(selector: attribute => attribute.HttpMethods)
            .Any(predicate: verb => verb is "PATCH" or "MERGE");
}