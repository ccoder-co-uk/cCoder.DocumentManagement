// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.DocumentManagement.Services.Processings;

internal sealed partial class FolderProcessingService
{
    private static void ValidateInputs(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllOnGet(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateFolderOnAdd(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateFolderOnUpdate(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateOrUpdateFolderOnAdd(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateOrUpdateForAppFolderOnAdd(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllFolderOnDelete(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateByAppIdOnDelete(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}