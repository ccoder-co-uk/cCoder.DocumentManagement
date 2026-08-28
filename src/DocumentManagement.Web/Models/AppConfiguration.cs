// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.DocumentManagement.Extensions;
using cCoder.DocumentManagement.Models;
using cCoder.Eventing.Models;
using cCoder.Security.Models;

namespace DocumentManagement.Web.Models;

public sealed class AppConfiguration
{
    public AppConfiguration()
    {
        CoreData = new CoreDataConfiguration();
        DocumentManagement =
            DocumentManagementConfigurationFactory
                .CreateDocumentManagementConfiguration();
        Eventing = new EventingConfiguration();
        Security = new SecurityConfiguration();
        SecurityData = new SecurityDataConfiguration();
    }

    public CoreDataConfiguration CoreData { get; set; }

    public DocumentManagementConfiguration DocumentManagement { get; set; }

    public EventingConfiguration Eventing { get; set; }

    public SecurityConfiguration Security { get; set; }

    public SecurityDataConfiguration SecurityData { get; set; }
}