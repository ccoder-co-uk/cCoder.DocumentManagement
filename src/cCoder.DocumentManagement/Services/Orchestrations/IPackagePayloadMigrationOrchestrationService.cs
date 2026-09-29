// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections.Generic;
using cCoder.DocumentManagement.Models;

namespace cCoder.DocumentManagement.Services.Orchestrations;

internal interface IPackagePayloadMigrationOrchestrationService
{
    FolderRoleInfo[] ParseFolderRoleInfos(string data);

    string SerializeFolderRoleInfos(
        IEnumerable<FolderRoleInfo> folderRoleInfos);
}