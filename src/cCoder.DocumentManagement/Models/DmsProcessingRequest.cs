// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using cCoder.Data.Models.CMS;


namespace cCoder.DocumentManagement.Models;

public class DmsProcessingRequest
{
    public App App { get; init; }
    public string Method { get; init; }
    public string RequestPath { get; init; }
    public string Host { get; init; }
    public string QueryString { get; init; }
    public string ContentType { get; init; }
    public Stream Body { get; init; }
    public Dictionary<string, string[]> Headers { get; init; }

}