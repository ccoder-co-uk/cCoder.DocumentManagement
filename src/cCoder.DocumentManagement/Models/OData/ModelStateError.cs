// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.DocumentManagement.Extensions.OData;

public sealed class ModelStateError
{
    public string Key { get; set; }
    public object Value { get; set; }
    public string[] Errors { get; set; }
}