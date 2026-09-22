// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.DocumentManagement.Models;
using cCoder.DocumentManagement.Models.Exceptions;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.DocumentManagement.Services.Foundations;
using Microsoft.EntityFrameworkCore;

namespace cCoder.DocumentManagement.Services.Processings;

internal partial class FolderRoleProcessingService(
    IFolderRoleService service)
    : IFolderRoleProcessingService
{
    public IQueryable<cCoder.Data.Models.Security.FolderRole> GetAll(bool ignoreFilters = false)
=>
        TryCatch(operation: () =>
        {
            ValidateAllOnGet(inputs: [ignoreFilters]);
            return service.GetAll(ignoreFilters: ignoreFilters);

        });

    public ValueTask<cCoder.Data.Models.Security.FolderRole> AddFolderRoleAsync(cCoder.Data.Models.Security.FolderRole newFolderRole)
=>
        TryCatch(operation: () =>
        {
            ValidateFolderRoleOnAdd(inputs: [newFolderRole]);

            if (service.CanCreateFolderRole(folderRole: newFolderRole))
            {
                if (service.FolderRoleExists(folderRole: newFolderRole))
                {
                    throw new DuplicateFolderRoleException();
                }

                return service.AddFolderRoleAsync(newFolderRole: newFolderRole);
            }


            throw new SecurityException(message: "Access Denied!");

        });

    public ValueTask DeleteFolderRoleAsync(cCoder.Data.Models.Security.FolderRole deletedFolderRole)
=>
        TryCatch(operation: async () =>
        {
            ValidateFolderRoleOnDelete(inputs: [deletedFolderRole]);

            cCoder.Data.Models.Security.FolderRole dbVersion = service.GetAll(ignoreFilters: true)
                .FirstOrDefault(predicate: (cCoder.Data.Models.Security.FolderRole ur) => ur.RoleId == deletedFolderRole.RoleId && ur.FolderId == deletedFolderRole.FolderId);


            if (dbVersion == null || !service.CanDeleteFolderRole(folderRole: deletedFolderRole))
            {
                throw new SecurityException(message: "Access Denied!");
            }


            await service.DeleteFolderRoleAsync(deletedFolderRole: dbVersion);

        });

    public ValueTask<IEnumerable<Result<cCoder.Data.Models.Security.FolderRole>>> AddOrUpdateFolderRole(IEnumerable<cCoder.Data.Models.Security.FolderRole> items)
=>
        TryCatch(operation: async () =>
        {
            ValidateOrUpdateFolderRoleOnAdd(inputs: [items]);
            cCoder.Data.Models.Security.FolderRole[] itemArray = items.ToArray();


            Guid[] leftIds = itemArray.Select(selector: (cCoder.Data.Models.Security.FolderRole item) => item.FolderId)
                .Distinct()
                .ToArray();


            cCoder.Data.Models.Security.FolderRole[] existingItems = (from item in GetAllValue()
                                                                      where ((ReadOnlySpan<Guid>)leftIds).Contains(value: item.FolderId)
                                                                      select item).ToArray();


            List<Result<cCoder.Data.Models.Security.FolderRole>> results = new List<Result<cCoder.Data.Models.Security.FolderRole>>();


            foreach (IGrouping<Guid, cCoder.Data.Models.Security.FolderRole> group in from item in itemArray
                                                                                      group item by item.FolderId)
            {
                cCoder.Data.Models.Security.FolderRole[] groupItems = group.ToArray();

                cCoder.Data.Models.Security.FolderRole[] existingGroupItems = existingItems.Where(predicate: (cCoder.Data.Models.Security.FolderRole item) => object.Equals(objA: item.FolderId, objB: group.Key))
                    .ToArray();

                await DeleteAllFolderRoleValueAsync(deletedFolderRole: existingGroupItems);

                foreach (cCoder.Data.Models.Security.FolderRole item in groupItems)
                {
                    try
                    {
                        results.Add(item: new Result<cCoder.Data.Models.Security.FolderRole>
                        {
                            Id = $"{item.FolderId}:{item.RoleId}",
                            Success = true,
                            Item = await AddFolderRoleValueAsync(newFolderRole: item),
                            Message = "Added Successfully"
                        });
                    }
                    catch (Exception ex)
                    {
                        results.Add(item: new Result<cCoder.Data.Models.Security.FolderRole>
                        {
                            Id = $"{item.FolderId}:{item.RoleId}",
                            Success = false,
                            Item = item,
                            Message = ex.Message
                        });
                    }
                }
            }


            return (IEnumerable<Result<cCoder.Data.Models.Security.FolderRole>>)results;

        });

    public ValueTask DeleteAllFolderRoleAsync(IEnumerable<cCoder.Data.Models.Security.FolderRole> deletedFolderRole)
=>
        TryCatch(operation: async () =>
        {
            ValidateAllFolderRoleOnDelete(inputs: [deletedFolderRole]);

            foreach (cCoder.Data.Models.Security.FolderRole item in deletedFolderRole)
            {
                await DeleteFolderRoleValueAsync(deletedFolderRole: item);
            }

        });

    private IQueryable<cCoder.Data.Models.Security.FolderRole> GetAllValue() =>
        GetAll();

    private ValueTask DeleteAllFolderRoleValueAsync(
        IEnumerable<cCoder.Data.Models.Security.FolderRole> deletedFolderRole) =>
        DeleteAllFolderRoleAsync(deletedFolderRole: deletedFolderRole);

    private ValueTask<cCoder.Data.Models.Security.FolderRole> AddFolderRoleValueAsync(
        cCoder.Data.Models.Security.FolderRole newFolderRole) =>
        AddFolderRoleAsync(newFolderRole: newFolderRole);

    private ValueTask DeleteFolderRoleValueAsync(
        cCoder.Data.Models.Security.FolderRole deletedFolderRole) =>
        DeleteFolderRoleAsync(deletedFolderRole: deletedFolderRole);
}