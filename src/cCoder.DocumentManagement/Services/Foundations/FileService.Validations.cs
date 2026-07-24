// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Dependencies;
using LocalFile = cCoder.Data.Models.DMS.File;

namespace cCoder.DocumentManagement.Services.Foundations;

internal sealed partial class FileService
{
    private static void ValidateInputs(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateAllOnGet(bool ignoreFilters) =>
        ValidationRulesEngine.Validate(inputs: [ignoreFilters]);

    private static void ValidateIdsByFolderIdsOnGet(Guid[] folderIds, bool ignoreFilters) =>
        ValidationRulesEngine.Validate(inputs: [folderIds, ignoreFilters]);

    private static void ValidateWithFolderAndContentsOnGet(Guid fileId, bool ignoreFilters) =>
        ValidationRulesEngine.Validate(inputs: [fileId, ignoreFilters]);

    private static void ValidateWithFolderRolesAndContentsOnGet(Guid fileId, bool ignoreFilters) =>
        ValidationRulesEngine.Validate(inputs: [fileId, ignoreFilters]);

    private static void ValidateByPathOnGet(int appId, string path, bool ignoreFilters) =>
        ValidationRulesEngine.Validate(inputs: [appId, path, ignoreFilters]);

    private static void ValidateByPathWithFolderAndContentsOnGet(int appId, string path, bool ignoreFilters) =>
        ValidationRulesEngine.Validate(inputs: [appId, path, ignoreFilters]);

    private static void ValidateByPathWithFolderRolesAndContentsOnGet(int appId, string path, bool ignoreFilters) =>
        ValidationRulesEngine.Validate(inputs: [appId, path, ignoreFilters]);

    private static void ValidateFileOnAdd(LocalFile file) =>
        ValidationRulesEngine.Validate(inputs: [file]);

    private static void ValidateFileOnUpdate(LocalFile file) =>
        ValidationRulesEngine.Validate(inputs: [file]);

    private static void ValidateForAppFileOnUpdate(LocalFile file) =>
        ValidationRulesEngine.Validate(inputs: [file]);

    private static void ValidateAllForAppFileOnDelete(IEnumerable<LocalFile> items) =>
        ValidationRulesEngine.Validate(inputs: [items]);
}
