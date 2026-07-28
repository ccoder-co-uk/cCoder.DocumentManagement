// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;

namespace cCoder.DocumentManagement.Models;

public class DocumentManagementConfiguration
{
    public string ConnectionString { get; set; }
    public int? SslPort { get; set; }
    public bool DebugInfo { get; set; }
    public bool LogSQL { get; set; }
    public string RootPath { get; set; }
    public EventProvider[] EventProviders { get; set; }

    public DocumentManagementConfiguration()
    {
        ConnectionString = string.Empty;
        RootPath = "Api/DocumentManagement";
        EventProviders = [];
    }

}