// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Dependencies;
using cCoder.Data.Models.Security;

namespace cCoder.DocumentManagement.Services.Foundations;

internal sealed partial class FolderRoleService
{
    private static void ValidateInputs(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateAllOnGet(bool ignoreFilters) =>
        ValidationRulesEngine.Validate(inputs: [ignoreFilters]);

    private static void ValidateFolderRoleOnAdd(FolderRole newFolderRole) =>
        ValidationRulesEngine.Validate(inputs: [newFolderRole]);

    private static void ValidateFolderRoleOnDelete(FolderRole deletedFolderRole) =>
        ValidationRulesEngine.Validate(inputs: [deletedFolderRole]);
}
