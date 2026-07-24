// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.DMS;

namespace cCoder.DocumentManagement.Services.Orchestrations;

internal interface IPackagePayloadMigrationOrchestrationService
{
    FolderRoleInfo[] ParseFolderRoleInfos(string data);

    string SerializeFolderRoleInfos(
        IEnumerable<FolderRoleInfo> folderRoleInfos);
}