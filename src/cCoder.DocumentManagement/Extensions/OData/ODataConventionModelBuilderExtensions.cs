// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using cCoder.DocumentManagement.Brokers.OData;

namespace cCoder.DocumentManagement.Extensions.OData;

public static class ODataConventionModelBuilderExtensions
{
    public static void ConfigureDocumentManagementApiModel(
        this ODataConventionModelBuilder builder) =>
        new DocumentManagementModelBroker(
            builder: builder)
            .Configure();

    public static IEdmModel CreateIEdmModel() =>
        new DocumentManagementModelBroker()
            .Build()
            .EDMModel;
}