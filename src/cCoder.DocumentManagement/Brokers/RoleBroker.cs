using cCoder.Data;
using cCoder.Data.Models.Security;
using Microsoft.EntityFrameworkCore;

namespace cCoder.DocumentManagement.Brokers;

public interface IRoleBroker
{
    IQueryable<Role> GetAllRoles(bool ignoreFilters);
    ValueTask<Role> AddRoleAsync(Role entity);
    ValueTask<Role> UpdateRoleAsync(Role entity);
    ValueTask<int> DeleteRoleAsync(Role entity);
    ValueTask DeleteAllRolesAsync(IEnumerable<Role> items);
    int? GetAppId(Role entity);
}

internal class RoleBroker(ICoreContextFactory coreContextFactory) : IRoleBroker
{
    public IQueryable<Role> GetAllRoles(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        return ignoreFilters
            ? coreDataContext.Roles.IgnoreQueryFilters()
            : coreDataContext.Roles;
    }

    public async ValueTask<Role> AddRoleAsync(Role entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Role result = (await coreDataContext.Roles.AddAsync(entity)).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<Role> UpdateRoleAsync(Role entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Role result = coreDataContext.Roles.Update(entity).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteRoleAsync(Role entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Roles.Remove(entity);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllRolesAsync(IEnumerable<Role> items)
    {
        if (items == null || !items.Any())
            return;

        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Roles.RemoveRange(items);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public int? GetAppId(Role entity) => entity.AppId;
}


