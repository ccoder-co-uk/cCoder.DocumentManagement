// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;

namespace cCoder.DocumentManagement;

public static class DocumentManagementConfigurationExtensions
{
    public static DocumentManagementConfiguration WithEventProviders(
        this DocumentManagementConfiguration documentManagementConfiguration,
        params EventProvider[] eventProviders)
    {
        documentManagementConfiguration.EventProviders = eventProviders ?? [];
        return documentManagementConfiguration;
    }
}