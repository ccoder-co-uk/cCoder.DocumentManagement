// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models.OData;

namespace cCoder.DocumentManagement.Extensions.OData;

internal static class ODataMetadataProvider
{
    internal static object GetMetadata(Type type, bool isEntity, bool hasEndpoint) =>
        new MetadataContainer(type: type, isEntity: isEntity, hasEndpoint: hasEndpoint);
}