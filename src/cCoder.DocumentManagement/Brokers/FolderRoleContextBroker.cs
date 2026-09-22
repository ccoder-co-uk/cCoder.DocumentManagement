// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Dependencies;
using cCoder.DocumentManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace cCoder.DocumentManagement.Brokers;

public interface IFolderRoleContextBroker
{
    FolderRoleContext SelectFolderRoleContext(
        FolderRole folderRole,
        bool ignoreFilters);
}

internal sealed class FolderRoleContextBroker(
    ICoreContextFactory coreContextFactory)
    : IFolderRoleContextBroker
{
    public FolderRoleContext SelectFolderRoleContext(
        FolderRole folderRole,
        bool ignoreFilters)
    {
        CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        Func<IQueryable<Folder>>[] folderQuerySelectors =
        [
            () => coreDataContext.Folders,
            () => coreDataContext.Folders.IgnoreQueryFilters(),
        ];

        Func<IQueryable<Role>>[] roleQuerySelectors =
        [
            () => coreDataContext.Roles,
            () => coreDataContext.Roles.IgnoreQueryFilters(),
        ];

        IQueryable<Folder> folders =
            folderQuerySelectors[Convert.ToInt32(value: ignoreFilters)]();

        IQueryable<Role> roles =
            roleQuerySelectors[Convert.ToInt32(value: ignoreFilters)]();

        return new FolderRoleContext
        {
            Folder = folders
                .FirstOrDefault(
                    predicate: folder =>
                        folder.Id == folderRole.FolderId),
            Role = roles
                .FirstOrDefault(
                    predicate: role =>
                        role.Id == folderRole.RoleId),
        };
    }
}