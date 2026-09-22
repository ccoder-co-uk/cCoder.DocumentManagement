// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.DocumentManagement.Services.Orchestrations;

internal sealed partial class FileContentOrchestrationService
{
    private static void ValidateInputs(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllOnGet(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateFileContentOnAdd(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateFileContentOnUpdate(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateOrUpdateFileContentOnAdd(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllFileContentOnDelete(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}