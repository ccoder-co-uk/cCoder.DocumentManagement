// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.DMS;

namespace cCoder.DocumentManagement.Services.Foundations;

internal sealed partial class FileContentService
{
    private static void ValidateInputs(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllOnGet(bool ignoreFilters) =>
        Validate(inputs: [ignoreFilters]);

    private static void ValidateAllForFileOnDelete(Guid fileId) =>
        Validate(inputs: [fileId]);

    private static void ValidateAllForFilesOnDelete(Guid[] fileIds) =>
        Validate(inputs: [fileIds]);

    private static void ValidateFileContentOnAdd(FileContent newFileContent) =>
        Validate(inputs: [newFileContent]);

    private static void ValidateFileContentOnUpdate(FileContent updatedFileContent) =>
        Validate(inputs: [updatedFileContent]);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}