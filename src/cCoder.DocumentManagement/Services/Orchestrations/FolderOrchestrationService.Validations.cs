// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.DocumentManagement.Services.Orchestrations;

internal sealed partial class FolderOrchestrationService
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

    private static void ValidateAllByAppIdOnDelete(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}