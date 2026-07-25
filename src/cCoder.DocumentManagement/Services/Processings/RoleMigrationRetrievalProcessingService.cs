// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Foundations;

namespace cCoder.DocumentManagement.Services.Processings;

internal sealed partial class RoleMigrationRetrievalProcessingService(
    IRoleService roleService)
    : IRoleMigrationRetrievalProcessingService
{
    public IQueryable<Role> GetAllRoles(bool ignoreFilters) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [ignoreFilters]);

            return roleService.GetAll(
                ignoreFilters: ignoreFilters);
        });
}