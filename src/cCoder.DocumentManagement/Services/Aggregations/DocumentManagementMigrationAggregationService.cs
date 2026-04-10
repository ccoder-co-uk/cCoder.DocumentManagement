using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Foundations;
using cCoder.DocumentManagement.Services.Orchestrations;
using IJsonBroker = cCoder.DocumentManagement.Brokers.IJsonBroker;
using LocalFolderRole = cCoder.Data.Models.Security.FolderRole;


namespace cCoder.DocumentManagement.Services.Aggregations;

internal class DocumentManagementMigrationAggregationService(
    IFolderRoleOrchestrationService folderRoleOrchestrationService,
    IFolderOrchestrationService folderOrchestrationService,
    IRoleService roleService,
    IJsonBroker jsonBroker
) : IDocumentManagementMigrationAggregationService
{
    public async ValueTask ImportPackageAsync(int appId, DocumentManagementPackage package)
    {
        if (package.Items is null || package.Items.Count == 0)
            return;

        foreach (DocumentManagementPackageItem item in package.Items)
        {
            if (item.Type != "Core/FolderRole")
                continue;

            FolderRoleInfo[] folderRoleInfos = item.Data.StartsWith("{")
                ? [jsonBroker.ParseJson<FolderRoleInfo>(item.Data)]
                : jsonBroker.ParseJson<FolderRoleInfo[]>(item.Data);

            var roles = roleService.GetAll(false).Where(role => role.AppId == appId).ToArray();
            var folders = folderOrchestrationService
                .GetAll(false)
                .Where(folder => folder.AppId == appId)
                .ToArray();

            List<LocalFolderRole> folderRolesToAdd = [];

            foreach (FolderRoleInfo folderRoleInfo in folderRoleInfos)
            {
                var folder = folders.FirstOrDefault(existing => existing.Path == folderRoleInfo.Path);
                var role = roles.FirstOrDefault(existing => existing.Name == folderRoleInfo.Name);

                if (folder is not null && role is not null)
                    folderRolesToAdd.Add(new LocalFolderRole { FolderId = folder.Id, RoleId = role.Id });
            }

        _ = await folderRoleOrchestrationService.AddOrUpdate(folderRolesToAdd);
        }
    }

    public DocumentManagementPackage ExportPackage(int appId, string packageName)
    {
        var package = packageName == "FolderRoles"
            ? ExportFolderRoles(appId)
            : new Data.Models.Packaging.Package(packageName) { Items = [] };

        return new DocumentManagementPackage(package.Name)
        {
            Id = package.Id,
            Name = package.Name,
            Description = package.Description,
            Category = package.Category,
            SourceApi = package.SourceApi,
            Items = package.Items?
                .Select(item => new DocumentManagementPackageItem
                {
                    Id = item.Id,
                    PackageId = item.PackageId,
                    Type = item.Type,
                    Data = item.Data,
                })
                .ToArray(),
        };
    }

    private cCoder.Data.Models.Packaging.Package ExportFolderRoles(int appId)
    {
        var roles = roleService.GetAll(true)
            .Where(role => role.AppId == appId)
            .Select(role => new { role.Id, role.Name })
            .ToArray();

        var folders = folderOrchestrationService.GetAll(true)
            .Where(folder => folder.AppId == appId)
            .Select(folder => new { folder.Id, folder.Path })
            .ToArray();

        if (roles.Length == 0 || folders.Length == 0)
        {
            return new Data.Models.Packaging.Package("FolderRoles")
            {
                Items =
                [
                    new Data.Models.Packaging.PackageItem
                    {
                        Type = "Core/FolderRole",
                        Data = jsonBroker.Serialize(Array.Empty<FolderRoleInfo>()),
                    },
                ],
            };
        }

        var roleNamesById = roles.ToDictionary(role => role.Id, role => role.Name);
        var folderPathsById = folders.ToDictionary(folder => folder.Id, folder => folder.Path);
        Guid[] roleIds = roleNamesById.Keys.ToArray();
        Guid[] folderIds = folderPathsById.Keys.ToArray();

        var folderRoles = folderRoleOrchestrationService.GetAll(true)
            .Where(folderRole =>
                folderIds.Contains(folderRole.FolderId)
                && roleIds.Contains(folderRole.RoleId))
            .ToArray();

        FolderRoleInfo[] folderRoleInfos = folderRoles
            .Select(folderRole => new FolderRoleInfo
            {
                Path = folderPathsById[folderRole.FolderId],
                Name = roleNamesById[folderRole.RoleId],
            })
            .ToArray();

        return new Data.Models.Packaging.Package("FolderRoles")
        {
            Items =
            [
                new Data.Models.Packaging.PackageItem
                {
                    Type = "Core/FolderRole",
                    Data = jsonBroker.Serialize(folderRoleInfos),
                },
            ],
        };
    }
}






