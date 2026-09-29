// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.IO;

namespace cCoder.DocumentManagement.Models;

public class DMSResult
{
    public string MimeType { get; set; }

    public Stream Data { get; set; }
}