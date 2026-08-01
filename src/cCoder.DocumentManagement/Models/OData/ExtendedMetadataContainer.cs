// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.DocumentManagement.Models.OData;

public class ExtendedMetadataContainer : MetadataContainer
{
    public IEnumerable<OperationContainer> Operations { get; set; }

    public ExtendedMetadataContainer() { }

    public ExtendedMetadataContainer(Type type, bool isEntity = false, bool hasEndpoint = false)
        : base(type: type, isEntity: isEntity, hasEndpoint: hasEndpoint) { }
}