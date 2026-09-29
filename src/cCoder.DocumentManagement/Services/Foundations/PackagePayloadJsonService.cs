// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using cCoder.DocumentManagement.Models;
using cCoder.DocumentManagement.Brokers;

namespace cCoder.DocumentManagement.Services.Foundations;

internal sealed partial class PackagePayloadJsonService(
    IJsonBroker jsonBroker)
    : IPackagePayloadJsonService
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