// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;

namespace cCoder.DocumentManagement.Services.Orchestrations;

internal interface IRoleMigrationOrchestrationService
{
    Role[] GetRolesForApp(int appId, bool ignoreFilters);
}