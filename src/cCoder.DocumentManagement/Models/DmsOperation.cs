// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using DmsFile = cCoder.Data.Models.DMS.File;

namespace cCoder.DocumentManagement.Models;

public class DmsOperation
{
    public IEnumerable<string> Paths { get; init; }

    public string Path { get; init; }

    public string NewPath { get; init; }

    public int Version { get; init; }

    public string Search { get; init; }

    public string Needle { get; init; }

    public Stream Content { get; init; }

    public bool IgnoreArchiveRoot { get; init; }

    public DMSResult Result { get; set; }

    public IEnumerable<DmsFile> Files { get; set; }
}