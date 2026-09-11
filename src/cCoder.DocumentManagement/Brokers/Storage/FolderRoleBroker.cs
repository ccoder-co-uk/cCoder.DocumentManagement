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
    int? SelectAppId(FolderRole folderRole);
}

internal sealed class FolderRoleBroker(ICoreContextFactory coreContextFactory) : IFolderRoleBroker
{

    public IQueryable<FolderRole> SelectAllFolderRoles(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        IQueryable<Guid> accessibleFolderIds = coreDataContext.Folders
            .ApplyQueryFilters(ignoreFilters: ignoreFilters)
            .Select(selector: folder => folder.Id);

        return coreDataContext.FolderRoles
            .IgnoreQueryFilters()
            .Where(predicate: folderRole => accessibleFolderIds.Contains(value: folderRole.FolderId));
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

    public int? SelectAppId(FolderRole folderRole)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.Folders

            .Where(predicate: folder => folder.Id == folderRole.FolderId)
            .Select(selector: folder => (int?)folder.AppId)
            .FirstOrDefault();

    }
}