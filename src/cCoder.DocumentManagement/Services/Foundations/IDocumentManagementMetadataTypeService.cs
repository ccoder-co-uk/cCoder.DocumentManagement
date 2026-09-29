// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections.Generic;

using cCoder.DocumentManagement.Models.OData;


namespace cCoder.DocumentManagement.Services.Foundations;

internal interface IDocumentManagementMetadataTypeService
{
    IEnumerable<MetadataContainerSet> GetKnownMetadata();
}