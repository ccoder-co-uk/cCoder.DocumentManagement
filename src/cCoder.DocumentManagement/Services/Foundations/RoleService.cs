// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using IRoleBroker = cCoder.DocumentManagement.Brokers.IRoleBroker;


namespace cCoder.DocumentManagement.Services.Foundations;

internal partial class RoleService(IRoleBroker roleBroker) : IRoleService
{
    public IQueryable<Role> GetAll(bool ignoreFilters = false)
=>
        TryCatch(operation: () =>
        {
            ValidateAllOnGet(ignoreFilters: ignoreFilters);
            return roleBroker.GetAllRoles(ignoreFilters: ignoreFilters);
        });
}