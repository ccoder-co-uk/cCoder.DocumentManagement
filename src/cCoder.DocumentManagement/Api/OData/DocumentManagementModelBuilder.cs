// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq.Expressions;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Models;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using FileModel = cCoder.Data.Models.DMS.File;

namespace cCoder.DocumentManagement.Api.OData;

internal class DocumentManagementModelBuilder : ODataModelBuilder
{
    public DocumentManagementModelBuilder(ODataConventionModelBuilder builder = null)
        : base(builder: builder)
    {
    }

    public override ODataModel Build()
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
        return base.Builder.GetEdmModel();
    }

    private void ConfigureModel()
    {
        AddCommonComplextypes();
        AddSet<FileModel, Guid>();
        AddSet<Folder, Guid>();
        AddSet<FileContent, Guid>();
        AddJoinSet(key: (Expression<Func<FolderRole, object>>)((FolderRole i) => new { i.FolderId, i.RoleId }));
        base.Builder.Namespace = "";
        base.Builder.EntityType<Folder>().Collection.Action(name: "Copy").ReturnsCollection<Result<Guid?>>();
    }
}