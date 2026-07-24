// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.DocumentManagement.Dependencies;
using cCoder.Data.Models.Security;
using Microsoft.EntityFrameworkCore;


namespace cCoder.DocumentManagement.Brokers.Storage;

public interface IFolderRoleBroker
{
    IQueryable<FolderRole> SelectAllFolderRoles(bool ignoreFilters);
    ValueTask<FolderRole> InsertFolderRoleAsync(FolderRole newFolderRole);
    ValueTask<int> DeleteFolderRoleAsync(FolderRole deletedFolderRole);
    ValueTask DeleteAllFolderRolesAsync(IEnumerable<FolderRole> deletedFolderRole);
    int? SelectAppId(FolderRole entity);
}

internal sealed class FolderRoleBroker(ICoreContextFactory coreContextFactory) : IFolderRoleBroker
{

    public IQueryable<FolderRole> SelectAllFolderRoles(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return Branching.ApplyQueryFilters(query: coreDataContext.FolderRoles, ignoreFilters: ignoreFilters);
    }

    public async ValueTask<FolderRole> InsertFolderRoleAsync(FolderRole newFolderRole)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        FolderRole result = (await coreDataContext.FolderRoles.AddAsync(entity: newFolderRole)).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteFolderRoleAsync(FolderRole deletedFolderRole)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.FolderRoles.Remove(entity: deletedFolderRole);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllFolderRolesAsync(IEnumerable<FolderRole> deletedFolderRole)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.FolderRoles.RemoveRange(entities: deletedFolderRole ?? []);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public int? SelectAppId(FolderRole entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.Folders

            .Where(predicate: folder => folder.Id == entity.FolderId)
            .Select(selector: folder => (int?)folder.AppId)
            .FirstOrDefault();

    }
}
