// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;


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

    public DmsProcessingRequest()
    {
        Method = string.Empty;
        RequestPath = string.Empty;
        Host = string.Empty;
        QueryString = string.Empty;
        ContentType = string.Empty;
        Body = Stream.Null;
        Headers = new(comparer: StringComparer.OrdinalIgnoreCase);
    }
}