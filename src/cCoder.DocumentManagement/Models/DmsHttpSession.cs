// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.DocumentManagement.Models;

public sealed class DmsHttpSession
{
    public HttpContext HttpContext { get; init; }
    public App App { get; set; }
    public DmsProcessingRequest Request { get; set; }
    public DmsProcessingResponse Response { get; set; }
}