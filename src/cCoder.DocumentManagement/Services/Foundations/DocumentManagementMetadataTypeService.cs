using cCoder.DocumentManagement.Api.OData;
using cCoder.Data.Models.DMS;
using DmsFile = cCoder.Data.Models.DMS.File;
using FolderRole = cCoder.Data.Models.Security.FolderRole;


namespace cCoder.DocumentManagement.Services.Foundations;

internal sealed class DocumentManagementMetadataTypeService : IDocumentManagementMetadataTypeService
{
    public IEnumerable<MetadataContainerSet> GetKnownMetadata() =>
    [
        new MetadataContainerSet
        {
            Name = "DocumentManagement",
            UriBase = "DocumentManagement",
            Types =
            [
                Entity<DmsFile>(),
                Entity<FileContent>(),
                Entity<Folder>(),
                Entity<FolderRole>(),
            ],
        },
    ];

    private static ExtendedMetadataContainer Entity<T>() =>
        new(typeof(T), isEntity: true, hasEndpoint: true)
        {
            Category = "DocumentManagement",
        };
}

