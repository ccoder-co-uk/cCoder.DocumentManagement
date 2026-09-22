// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.DocumentManagement.Services.Processings;

internal sealed partial class FileContentProcessingService
{
    private static void ValidateInputs(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllOnGet(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateFileContentOnAdd(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateFileContentOnUpdate(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllForFileOnDelete(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllForFilesOnDelete(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateOrUpdateFileContentOnAdd(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllFileContentOnDelete(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}