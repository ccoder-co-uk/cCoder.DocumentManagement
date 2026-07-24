// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Processings;

namespace cCoder.DocumentManagement.Services.Orchestrations;

internal sealed partial class RoleMigrationOrchestrationService(
    IRoleMigrationRetrievalProcessingService retrievalProcessingService,
    IRoleMigrationFilterProcessingService filterProcessingService)
    : IRoleMigrationOrchestrationService
{
    public Role[] GetRolesForApp(int appId, bool ignoreFilters) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [appId, ignoreFilters]);

            IQueryable<Role> roles =
                retrievalProcessingService.GetAllRoles(
                    ignoreFilters: ignoreFilters);

            return filterProcessingService.FilterRolesForApp(
                roles: roles,
                appId: appId);
        });
}