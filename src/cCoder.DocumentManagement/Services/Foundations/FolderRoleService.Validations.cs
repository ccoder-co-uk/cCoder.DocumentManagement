// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;

namespace cCoder.DocumentManagement.Services.Foundations;

internal sealed partial class FolderRoleService
{
    private static void ValidateInputs(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllOnGet(bool ignoreFilters) =>
        Validate(inputs: [ignoreFilters]);

    private static void ValidateFolderRoleOnAdd(FolderRole newFolderRole) =>
        Validate(inputs: [newFolderRole]);

    private static void ValidateFolderRoleOnDelete(FolderRole deletedFolderRole) =>
        Validate(inputs: [deletedFolderRole]);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}