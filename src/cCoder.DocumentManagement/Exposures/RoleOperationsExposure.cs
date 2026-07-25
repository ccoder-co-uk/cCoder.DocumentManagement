// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Foundations;

namespace cCoder.DocumentManagement.Exposures;

internal sealed class RoleOperationsExposure(
    IRoleService roleService)
    : IRoleOperationsExposure
{
    public IQueryable<Role> GetAllRoles(bool ignoreFilters = false) =>
        roleService.GetAll(
            ignoreFilters: ignoreFilters);
}