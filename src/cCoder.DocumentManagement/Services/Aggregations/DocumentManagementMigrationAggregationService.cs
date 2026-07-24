// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Foundations;
using cCoder.DocumentManagement.Services.Orchestrations;
using IJsonBroker = cCoder.DocumentManagement.Brokers.IJsonBroker;
using LocalFolderRole = cCoder.Data.Models.Security.FolderRole;


namespace cCoder.DocumentManagement.Services.Aggregations;

internal partial class DocumentManagementMigrationAggregationService(
    IFolderRoleOrchestrationService folderRoleOrchestrationService,
    IFolderOrchestrationService folderOrchestrationService,
    IRoleService roleService,
    IJsonBroker jsonBroker
) : IDocumentManagementMigrationAggregationService
{
    public ValueTask ImportPackageAsync(int appId, DocumentManagementPackage package)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [appId, package]);
            if (package.Items is null || package.Items.Count == 0)
            {
                return;
            }


            foreach (DocumentManagementPackageItem item in package.Items)
            {
                if (item.Type != "Core/FolderRole")
                {
                    continue;
                }

                FolderRoleInfo[] folderRoleInfos = item.Data.StartsWith(value: "{")
                    ? [jsonBroker.ParseJson<FolderRoleInfo>(json: item.Data)]
                    : jsonBroker.ParseJson<FolderRoleInfo[]>(json: item.Data);

                var roles = roleService.GetAll(ignoreFilters: false)
                    .Where(predicate: role => role.AppId == appId)
                    .ToArray();

                var folders = folderOrchestrationService
                    .GetAll(ignoreFilters: false)
                    .Where(predicate: folder => folder.AppId == appId)
                    .ToArray();

                List<LocalFolderRole> folderRolesToAdd = [];

                foreach (FolderRoleInfo folderRoleInfo in folderRoleInfos)
                {
                    var folder = folders.FirstOrDefault(predicate: existing => existing.Path == folderRoleInfo.Path);
                    var role = roles.FirstOrDefault(predicate: existing => existing.Name == folderRoleInfo.Name);

                    if (folder is not null && role is not null)
                    {
                        folderRolesToAdd.Add(item: new LocalFolderRole { FolderId = folder.Id, RoleId = role.Id });
                    }
                }

                _ = await folderRoleOrchestrationService.AddOrUpdate(items: folderRolesToAdd);
            }

        });

    public DocumentManagementPackage ExportPackage(int appId, string packageName)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [appId, packageName]);
            var package = packageName == "FolderRoles"
    ? ExportFolderRoles(appId: appId)
    : new Data.Models.Packaging.Package(name: packageName) { Items = [] };


            return new DocumentManagementPackage(name: package.Name)
            {
                Id = package.Id,
                Name = package.Name,
                Description = package.Description,
                Category = package.Category,
                SourceApi = package.SourceApi,
                Items = package.Items?
                    .Select(selector: item => new DocumentManagementPackageItem
                    {
                        Id = item.Id,
                        PackageId = item.PackageId,
                        Type = item.Type,
                        Data = item.Data,
                    })
                    .ToArray(),
            };

        });

    private cCoder.Data.Models.Packaging.Package ExportFolderRoles(int appId)
    {
        var roles = roleService.GetAll(ignoreFilters: true)
            .Where(predicate: role => role.AppId == appId)
            .Select(selector: role => new { role.Id, role.Name })
            .ToArray();

        var folders = folderOrchestrationService.GetAll(ignoreFilters: true)
            .Where(predicate: folder => folder.AppId == appId)
            .Select(selector: folder => new { folder.Id, folder.Path })
            .ToArray();

        if (roles.Length == 0 || folders.Length == 0)
        {
            return new Data.Models.Packaging.Package(name: "FolderRoles")
            {
                Items =
                [
                    new Data.Models.Packaging.PackageItem
                    {
                        Type = "Core/FolderRole",
                        Data = jsonBroker.Serialize(value:Array.Empty<FolderRoleInfo>()),
                    },
                ],
            };
        }

        var roleNamesById = roles.ToDictionary(keySelector: role => role.Id, elementSelector: role => role.Name);
        var folderPathsById = folders.ToDictionary(keySelector: folder => folder.Id, elementSelector: folder => folder.Path);
        Guid[] roleIds = roleNamesById.Keys.ToArray();
        Guid[] folderIds = folderPathsById.Keys.ToArray();

        var folderRoles = folderRoleOrchestrationService.GetAll(ignoreFilters: true)
            .Where(predicate: folderRole =>
                folderIds.Contains(value: folderRole.FolderId)
                && roleIds.Contains(value: folderRole.RoleId))
            .ToArray();

        FolderRoleInfo[] folderRoleInfos = folderRoles
            .Select(selector: folderRole => new FolderRoleInfo
            {
                Path = folderPathsById[key: folderRole.FolderId],
                Name = roleNamesById[key: folderRole.RoleId],
            })
            .ToArray();

        return new Data.Models.Packaging.Package(name: "FolderRoles")
        {
            Items =
            [
                new Data.Models.Packaging.PackageItem
                {
                    Type = "Core/FolderRole",
                    Data = jsonBroker.Serialize(value:folderRoleInfos),
                },
            ],
        };
    }
}