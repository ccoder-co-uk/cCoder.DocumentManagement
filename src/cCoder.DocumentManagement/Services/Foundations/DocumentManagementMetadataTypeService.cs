// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Api.OData;
using cCoder.DocumentManagement.Dependencies.OData;
using cCoder.Data.Models.DMS;
using DmsFile = cCoder.Data.Models.DMS.File;
using FolderRole = cCoder.Data.Models.Security.FolderRole;


namespace cCoder.DocumentManagement.Services.Foundations;

internal sealed partial class DocumentManagementMetadataTypeService : IDocumentManagementMetadataTypeService
{
    public IEnumerable<MetadataContainerSet> GetKnownMetadata()
=>
        TryCatch(operation: IEnumerable<MetadataContainerSet> () =>
        {

            return [
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
        });

    private static ExtendedMetadataContainer Entity<T>() =>
        new(type: typeof(T), isEntity: true, hasEndpoint: true)
        {
            Category = "DocumentManagement",
        };
}
