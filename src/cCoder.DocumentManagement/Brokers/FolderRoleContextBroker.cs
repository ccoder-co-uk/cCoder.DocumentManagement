// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
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

        return new FolderRoleContext
        {
            Folder = Branching.ApplyQueryFilters(
                    query: coreDataContext.Folders,
                    ignoreFilters: ignoreFilters)
                .FirstOrDefault(
                    predicate: folder =>
                        folder.Id == folderRole.FolderId),
            Role = Branching.ApplyQueryFilters(
                    query: coreDataContext.Roles,
                    ignoreFilters: ignoreFilters)
                .FirstOrDefault(
                    predicate: role =>
                        role.Id == folderRole.RoleId),
        };
    }
}