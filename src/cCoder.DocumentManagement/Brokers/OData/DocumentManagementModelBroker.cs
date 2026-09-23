// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq.Expressions;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Models;
using cCoder.DocumentManagement.Models.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using FileModel = cCoder.Data.Models.DMS.File;

namespace cCoder.DocumentManagement.Brokers.OData;

internal class DocumentManagementModelBroker : IDocumentManagementModelBroker
{
    private readonly ODataConventionModelBuilder builder;

    public DocumentManagementModelBroker(ODataConventionModelBuilder builder = null)
    {
        this.builder = builder ?? new ODataConventionModelBuilder();
    }

    public ODataModel Build()
    {
        return new ODataModel
        {
            Context = "Core",
            Description = "Document Management endpoints for the platform.",
            EDMModel = BuildEdmModel()
        };
    }

    public void Configure()
    {
        ConfigureModel();
    }

    private IEdmModel BuildEdmModel()
    {
        ConfigureModel();
        return builder.GetEdmModel();
    }

    private void ConfigureModel()
    {
        AddCommonComplextypes();
        AddSet<FileModel, Guid>();
        AddSet<Folder, Guid>();
        AddSet<FileContent, Guid>();
        AddJoinSet(key: (Expression<Func<FolderRole, object>>)((FolderRole i) => new { i.FolderId, i.RoleId }));
        builder.Namespace = "";

        builder.EntityType<Folder>().Collection.Action(name: "Copy")
            .ReturnsCollection<Result<Guid?>>();
    }

    private EntitySetConfiguration<T> AddSet<T, TKey>(string setName = null)
        where T : class
    {
        setName ??= typeof(T).Name;

        return builder.EntitySet<T>(name: setName);
    }

    private EntitySetConfiguration<T> AddJoinSet<T, TKey>(
        Expression<Func<T, TKey>> key)
        where T : class
    {
        string name = typeof(T).Name;
        EntitySetConfiguration<T> result = builder.EntitySet<T>(name: name);

        builder.EntityType<T>()
            .HasKey(keyDefinitionExpression: key);

        return result;
    }

    private void AddCommonComplextypes()
    {
        builder.ComplexType<MetadataContainerSet>();
        builder.ComplexType<MetadataContainer>();
        builder.ComplexType<PropertyContainer>();
        builder.ComplexType<AuditResultsByUser>();
        builder.ComplexType<AuditResultByProperty>();
    }
}