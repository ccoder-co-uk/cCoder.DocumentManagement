// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.DocumentManagement.Services.Orchestrations;

internal sealed partial class DmsOrchestrationService
{
    private static void ValidateInputs(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateFilesZippedDmsOperationOnGet(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateDmsOperationOnGet(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}