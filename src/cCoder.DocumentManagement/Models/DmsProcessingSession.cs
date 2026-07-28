// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Dependencies;

namespace cCoder.DocumentManagement.Models;

public class DmsProcessingSession
{
    public DmsProcessingRequest Request { get; init; }

    public DmsProcessingResponse Response { get; set; }
}