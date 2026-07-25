// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.OData.Edm;


namespace cCoder.DocumentManagement.Api.OData;

public class ODataModel
{
    public string Context { get; set; }
    public string Description { get; set; }
    public IEdmModel EDMModel { get; set; }

    public ODataModel()
    {
        Context = string.Empty;
        Description = string.Empty;
    }
}