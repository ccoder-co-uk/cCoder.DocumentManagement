// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.DocumentManagement.Services.Foundations;

internal sealed partial class DmsInstanceService
{
    private static void ValidateInputs(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateFilesZippedOnGet(IEnumerable<string> paths) =>
        Validate(inputs: [paths]);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}