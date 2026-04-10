using cCoder.DocumentManagement.Api.OData;


namespace cCoder.DocumentManagement.Services.Foundations;

internal interface IDocumentManagementMetadataTypeService
{
    IEnumerable<MetadataContainerSet> GetKnownMetadata();
}

