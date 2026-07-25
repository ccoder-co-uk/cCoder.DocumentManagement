// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.DocumentManagement.Dependencies;

public class DmsProcessingResponse
{
    public Stream Body { get; init; }
    public string ContentType { get; init; }
    public int StatusCode { get; init; }
    public bool HasBody { get; init; }
    public List<KeyValuePair<string, string>> Headers { get; init; }

    public DmsProcessingResponse()
    {
        Body = Stream.Null;
        ContentType = string.Empty;
        Headers = [];
    }
}