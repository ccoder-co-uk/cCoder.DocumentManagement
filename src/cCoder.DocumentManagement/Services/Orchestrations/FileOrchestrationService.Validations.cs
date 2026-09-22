// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.DocumentManagement.Services.Orchestrations;

internal sealed partial class FileOrchestrationService
{
    private static void ValidateInputs(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllOnGet(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateFileOnAdd(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateFileOnUpdate(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateOrUpdateFileOnAdd(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllFileOnDelete(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateByPathOnGet(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}