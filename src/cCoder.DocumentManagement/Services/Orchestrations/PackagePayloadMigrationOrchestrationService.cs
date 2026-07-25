// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.DMS;
using cCoder.DocumentManagement.Services.Processings;

namespace cCoder.DocumentManagement.Services.Orchestrations;

internal sealed partial class PackagePayloadMigrationOrchestrationService(
    IPackagePayloadShapeProcessingService shapeProcessingService,
    IPackagePayloadJsonProcessingService jsonProcessingService)
    : IPackagePayloadMigrationOrchestrationService
{
    public FolderRoleInfo[] ParseFolderRoleInfos(string data) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [data]);

            return shapeProcessingService.IsSingleItem(
                data: data)
                    ?
                    [
                        jsonProcessingService.ParseFolderRoleInfo(
                            data: data),
                    ]
                    : jsonProcessingService.ParseFolderRoleInfos(
                        data: data);
        });

    public string SerializeFolderRoleInfos(
        IEnumerable<FolderRoleInfo> folderRoleInfos) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [folderRoleInfos]);

            return jsonProcessingService.SerializeFolderRoleInfos(
                folderRoleInfos: folderRoleInfos);
        });
}