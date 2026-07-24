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
    public required App App { get; init; }
    public required string Method { get; init; }
    public required string RequestPath { get; init; }
    public required string Host { get; init; }
    public string QueryString { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public Stream Body { get; init; } = Stream.Null;
    public Dictionary<string, string[]> Headers { get; init; } =
        new(comparer: StringComparer.OrdinalIgnoreCase);
}