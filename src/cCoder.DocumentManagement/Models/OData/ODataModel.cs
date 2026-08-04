// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.OData.Edm;


namespace cCoder.DocumentManagement.Extensions.OData;

public class ODataModel
{
    public string Context { get; set; }
    public string Description { get; set; }
    public IEdmModel EDMModel { get; set; }

}