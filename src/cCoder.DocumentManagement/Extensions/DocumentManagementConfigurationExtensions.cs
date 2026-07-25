// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;

namespace cCoder.DocumentManagement;

public static class DocumentManagementConfigurationExtensions
{
    public static DocumentManagementConfiguration WithEventProviders(
        this DocumentManagementConfiguration configuration,
        params EventProvider[] eventProviders)
    {
        configuration.EventProviders = eventProviders ?? [];
        return configuration;
    }
}