// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.DocumentManagement.Models.OData;

public class ExtendedMetadataContainer : MetadataContainer
{
    public IEnumerable<OperationContainer> Operations { get; set; }

}