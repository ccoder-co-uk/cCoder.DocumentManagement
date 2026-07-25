// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Dependencies;
using cCoder.Data.Models.DMS;

namespace cCoder.DocumentManagement.Services.Foundations;

internal sealed partial class FileContentService
{
    private static void ValidateInputs(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateAllOnGet(bool ignoreFilters) =>
        ValidationRulesEngine.Validate(inputs: [ignoreFilters]);

    private static void ValidateAllForFileOnDelete(Guid fileId) =>
        ValidationRulesEngine.Validate(inputs: [fileId]);

    private static void ValidateAllForFilesOnDelete(Guid[] fileIds) =>
        ValidationRulesEngine.Validate(inputs: [fileIds]);

    private static void ValidateFileContentOnAdd(FileContent newFileContent) =>
        ValidationRulesEngine.Validate(inputs: [newFileContent]);

    private static void ValidateFileContentOnUpdate(FileContent updatedFileContent) =>
        ValidationRulesEngine.Validate(inputs: [updatedFileContent]);
}