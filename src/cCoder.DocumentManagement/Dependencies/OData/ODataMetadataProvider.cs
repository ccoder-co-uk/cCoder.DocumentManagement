// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.DocumentManagement.Dependencies.OData;

internal static class ODataMetadataProvider
{
    internal static object GetMetadata(Type type, bool isEntity, bool hasEndpoint) =>
        new MetadataContainer(type: type, isEntity: isEntity, hasEndpoint: hasEndpoint);
}