// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.DocumentManagement.Models;

public class DmsProcessingResponse
{
    public Stream Body { get; init; } = Stream.Null;
    public required string ContentType { get; init; }
    public required int StatusCode { get; init; }
    public bool HasBody { get; init; }
    public List<KeyValuePair<string, string>> Headers { get; init; } = [];
}