// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.DMS;

namespace cCoder.DocumentManagement.Services.Processings;

internal interface IPackagePayloadJsonProcessingService
{
    FolderRoleInfo ParseFolderRoleInfo(string data);

    FolderRoleInfo[] ParseFolderRoleInfos(string data);

    string SerializeFolderRoleInfos(
        IEnumerable<FolderRoleInfo> folderRoleInfos);
}