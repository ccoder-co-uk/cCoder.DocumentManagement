// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.DMS;
using cCoder.DocumentManagement.Brokers;

namespace cCoder.DocumentManagement.Services.Processings;

internal sealed partial class PackagePayloadJsonProcessingService(
    IJsonBroker jsonBroker)
    : IPackagePayloadJsonProcessingService
{
    public FolderRoleInfo ParseFolderRoleInfo(string data) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [data]);

            return jsonBroker.ParseJson<FolderRoleInfo>(
                json: data);
        });

    public FolderRoleInfo[] ParseFolderRoleInfos(string data) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [data]);

            return jsonBroker.ParseJson<FolderRoleInfo[]>(
                json: data);
        });

    public string SerializeFolderRoleInfos(
        IEnumerable<FolderRoleInfo> folderRoleInfos) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [folderRoleInfos]);

            return jsonBroker.Serialize(
                value: folderRoleInfos.ToArray());
        });
}