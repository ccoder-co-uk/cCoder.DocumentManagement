// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.DocumentManagement.Models;
using cCoder.Eventing.Models;
using cCoder.Security.Models;

namespace DocumentManagement.Web.Models;

public sealed class DocumentManagementWebConfiguration
{
    public DocumentManagementConfiguration DocumentManagement { get; set; }

    public DataConfiguration Data { get; set; }

    public SecurityConfiguration Security { get; set; }

    public EventingConfiguration Eventing { get; set; }

    public DocumentManagementWebConfiguration()
    {
        DocumentManagement = new DocumentManagementConfiguration();
        Data = new DataConfiguration();
        Security = new SecurityConfiguration();
        Eventing = new EventingConfiguration();
    }
}