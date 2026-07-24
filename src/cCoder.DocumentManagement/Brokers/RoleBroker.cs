// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.Security;
using Microsoft.EntityFrameworkCore;

namespace cCoder.DocumentManagement.Brokers;

public interface IRoleBroker
{
    IQueryable<Role> GetAllRoles(bool ignoreFilters);
    ValueTask<Role> AddRoleAsync(Role newRole);
    ValueTask<Role> UpdateRoleAsync(Role updatedRole);
    ValueTask<int> DeleteRoleAsync(Role deletedRole);
    ValueTask DeleteAllRolesAsync(IEnumerable<Role> deletedRole);
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

    public async ValueTask<Role> AddRoleAsync(Role newRole)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Role result = (await coreDataContext.Roles.AddAsync(entity: newRole)).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<Role> UpdateRoleAsync(Role updatedRole)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        Role result = coreDataContext.Roles.Update(entity: updatedRole).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteRoleAsync(Role deletedRole)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Roles.Remove(entity: deletedRole);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllRolesAsync(IEnumerable<Role> deletedRole)
    {
        if (deletedRole == null || !deletedRole.Any())
        {
            return;
        }

        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Roles.RemoveRange(entities: deletedRole);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public int? GetAppId(Role entity) =>
        entity.AppId;
}