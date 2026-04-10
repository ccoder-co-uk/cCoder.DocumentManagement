using cCoder.Data.Models.Security;
using IRoleBroker = cCoder.DocumentManagement.Brokers.IRoleBroker;


namespace cCoder.DocumentManagement.Services.Foundations;

internal class RoleService(IRoleBroker roleBroker) : IRoleService
{
    public IQueryable<Role> GetAll(bool ignoreFilters = false) =>
        roleBroker.GetAllRoles(ignoreFilters);
}


