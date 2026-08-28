// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;

namespace cCoder.DocumentManagement.Models;

public class DocumentManagementConfiguration
{
    public int? SslPort { get; set; }
    public string RootPath { get; set; }
    public EventProvider[] EventProviders { get; set; }

}