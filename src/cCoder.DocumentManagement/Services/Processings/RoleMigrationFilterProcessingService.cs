// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;

namespace cCoder.DocumentManagement.Services.Processings;

internal sealed partial class RoleMigrationFilterProcessingService
    : IRoleMigrationFilterProcessingService
{
    public Role[] FilterRolesForApp(
        IEnumerable<Role> roles,
        int appId) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [roles, appId]);

            return roles
                .Where(predicate: role => role.AppId == appId)
                .ToArray();
        });
}