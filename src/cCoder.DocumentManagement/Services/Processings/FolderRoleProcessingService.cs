// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.DocumentManagement.Brokers;
using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Foundations;
using Microsoft.EntityFrameworkCore;

namespace cCoder.DocumentManagement.Services.Processings;

internal partial class FolderRoleProcessingService(IFolderRoleService service, IRoleBroker roleBroker, IFolderService folderService, IAuthorizationBroker authorizationBroker) : IFolderRoleProcessingService
{
    private cCoder.Data.Models.Security.User User => authorizationBroker.GetCurrentUser();

    public IQueryable<cCoder.Data.Models.Security.FolderRole> GetAll(bool ignoreFilters = false)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [ignoreFilters]);
            return service.GetAll(ignoreFilters: ignoreFilters);

        });

    public ValueTask<cCoder.Data.Models.Security.FolderRole> AddAsync(cCoder.Data.Models.Security.FolderRole entity)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [entity]);
            (cCoder.Data.Models.Security.Role, Folder) folderAndRole = GetFolderAndRole(entity: entity);

            cCoder.Data.Models.Security.Role role = folderAndRole.Item1;

            Folder folder = folderAndRole.Item2;

            bool flag = role != null && folder != null;

            Func<Folder, cCoder.Data.Models.Security.Role, bool> func = (Folder currentFolder, cCoder.Data.Models.Security.Role currentRole) => currentFolder.UserCan(user: User, privilege: "folderrole_create");


            if (flag && func(arg1: folder, arg2: role))
            {
                ICollection<cCoder.Data.Models.Security.FolderRole> roles = folder.Roles;
                return (roles == null || !roles.Any(predicate: (cCoder.Data.Models.Security.FolderRole r) => r.RoleId == role.Id)) ? service.AddAsync(folderRole: entity) : ValueTask.FromResult(result: entity);
            }


            if (role != null && folder != null)
            {
                ICollection<cCoder.Data.Models.Security.FolderRole> folders = role.Folders;

                if (folders != null && folders.Any(predicate: (cCoder.Data.Models.Security.FolderRole r) => r.FolderId == folder.Id))
                {
                    return ValueTask.FromResult(result: entity);
                }
            }


            throw new SecurityException(message: "Access Denied!");

        });

    public ValueTask DeleteAsync(cCoder.Data.Models.Security.FolderRole link)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [link]);
            Folder folder = folderService.GetAll(ignoreFilters: true)
    .FirstOrDefault(predicate: (Folder u) => u.Id == link.FolderId);


            cCoder.Data.Models.Security.FolderRole dbVersion = service.GetAll(ignoreFilters: true)
                .FirstOrDefault(predicate: (cCoder.Data.Models.Security.FolderRole ur) => ur.RoleId == link.RoleId && ur.FolderId == link.FolderId);


            if (dbVersion == null || folder == null || !folder.UserCan(user: User, privilege: "folderrole_delete"))
            {
                throw new SecurityException(message: "Access Denied!");
            }


            await service.DeleteAsync(folderRole: dbVersion);

        });

    public ValueTask<IEnumerable<Result<cCoder.Data.Models.Security.FolderRole>>> AddOrUpdate(IEnumerable<cCoder.Data.Models.Security.FolderRole> items)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [items]);
            cCoder.Data.Models.Security.FolderRole[] itemArray = items.ToArray();


            Guid[] leftIds = itemArray.Select(selector: (cCoder.Data.Models.Security.FolderRole item) => item.FolderId)
                .Distinct()
                .ToArray();


            cCoder.Data.Models.Security.FolderRole[] existingItems = (from item in GetAll()
                                                                      where ((ReadOnlySpan<Guid>)leftIds).Contains(value: item.FolderId)
                                                                      select item).ToArray();


            List<Result<cCoder.Data.Models.Security.FolderRole>> results = new List<Result<cCoder.Data.Models.Security.FolderRole>>();


            foreach (IGrouping<Guid, cCoder.Data.Models.Security.FolderRole> group in from item in itemArray
                                                                                      group item by item.FolderId)
            {
                cCoder.Data.Models.Security.FolderRole[] groupItems = group.ToArray();

                cCoder.Data.Models.Security.FolderRole[] existingGroupItems = existingItems.Where(predicate: (cCoder.Data.Models.Security.FolderRole item) => object.Equals(objA: item.FolderId, objB: group.Key))
                    .ToArray();

                await DeleteAllAsync(items: existingGroupItems);

                foreach (cCoder.Data.Models.Security.FolderRole item in groupItems)
                {
                    try
                    {
                        results.Add(item: new Result<cCoder.Data.Models.Security.FolderRole>
                        {
                            Id = $"{item.FolderId}:{item.RoleId}",
                            Success = true,
                            Item = await AddAsync(entity: item),
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


            return (IEnumerable<Result<FolderRole>>)results;

        });

    public ValueTask DeleteAllAsync(IEnumerable<cCoder.Data.Models.Security.FolderRole> items)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [items]);
            foreach (cCoder.Data.Models.Security.FolderRole item in items)
            {
                await DeleteAsync(link: item);
            }

        });

    private (cCoder.Data.Models.Security.Role role, Folder folder) GetFolderAndRole(cCoder.Data.Models.Security.FolderRole entity)
=>
        (role: roleBroker.GetAllRoles(ignoreFilters: true)
            .IgnoreQueryFilters()
            .FirstOrDefault(predicate: (cCoder.Data.Models.Security.Role r) => r.Id == entity.RoleId), folder: folderService.GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: (Folder u) => u.Id == entity.FolderId));
}