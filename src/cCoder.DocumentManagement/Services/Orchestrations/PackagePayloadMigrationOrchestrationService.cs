// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections.Generic;
using cCoder.DocumentManagement.Models;
using cCoder.DocumentManagement.Services.Foundations;

namespace cCoder.DocumentManagement.Services.Orchestrations;

internal sealed partial class PackagePayloadMigrationOrchestrationService(
    IPackagePayloadShapeService shapeProcessingService,
    IPackagePayloadJsonService jsonProcessingService)
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