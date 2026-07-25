// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;

namespace cCoder.DocumentManagement.Exposures;

public interface IRoleOperationsExposure
{
    IQueryable<Role> GetAllRoles(bool ignoreFilters = false);
}