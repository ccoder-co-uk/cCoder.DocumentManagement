// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Dependencies;
using cCoder.Data.Models.DMS;

namespace cCoder.DocumentManagement.Services.Foundations;

internal sealed partial class FolderService
{
    private static void ValidateInputs(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateAllOnGet(bool ignoreFilters) =>
        ValidationRulesEngine.Validate(inputs: [ignoreFilters]);

    private static void ValidateWithRolesOnGet(Guid folderId, bool ignoreFilters) =>
        ValidationRulesEngine.Validate(inputs: [folderId, ignoreFilters]);

    private static void ValidateForUpdateOnGet(Guid folderId, bool ignoreFilters) =>
        ValidationRulesEngine.Validate(inputs: [folderId, ignoreFilters]);

    private static void ValidateByPathOnGet(int appId, string path, bool ignoreFilters) =>
        ValidationRulesEngine.Validate(inputs: [appId, path, ignoreFilters]);

    private static void ValidateByPathWithRolesOnGet(int appId, string path, bool ignoreFilters) =>
        ValidationRulesEngine.Validate(inputs: [appId, path, ignoreFilters]);

    private static void ValidateByPathWithParentAndRolesOnGet(int appId, string path, bool ignoreFilters) =>
        ValidationRulesEngine.Validate(inputs: [appId, path, ignoreFilters]);

    private static void ValidateByPathWithRolesAndFilesAndContentsOnGet(int appId, string path, bool ignoreFilters) =>
        ValidationRulesEngine.Validate(inputs: [appId, path, ignoreFilters]);

    private static void ValidateByPathWithSubFoldersAndFilesOnGet(int appId, string path, bool ignoreFilters) =>
        ValidationRulesEngine.Validate(inputs: [appId, path, ignoreFilters]);

    private static void ValidateFolderOnAdd(Folder newFolder) =>
        ValidationRulesEngine.Validate(inputs: [newFolder]);

    private static void ValidateForPathBuildFolderOnAdd(Folder newFolder) =>
        ValidationRulesEngine.Validate(inputs: [newFolder]);

    private static void ValidateFolderOnUpdate(Folder updatedFolder) =>
        ValidationRulesEngine.Validate(inputs: [updatedFolder]);

    private static void ValidateForAppFolderOnUpdate(Folder updatedFolder) =>
        ValidationRulesEngine.Validate(inputs: [updatedFolder]);

    private static void ValidateAllForAppFolderOnDelete(IEnumerable<Folder> deletedFolder) =>
        ValidationRulesEngine.Validate(inputs: [deletedFolder]);

    private static void ValidateAllByAppIdOnDelete(int appId) =>
        ValidationRulesEngine.Validate(inputs: [appId]);
}
