// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;

namespace cCoder.DocumentManagement.Extensions;

public static class DocumentManagementConfigurationFactory
{
    public static DocumentManagementConfiguration CreateDocumentManagementConfiguration() =>
        new()
        {
            RootPath = "Api/DocumentManagement",
            EventProviders = []
        };
}