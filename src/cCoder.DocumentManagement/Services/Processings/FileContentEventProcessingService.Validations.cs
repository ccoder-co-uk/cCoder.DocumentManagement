// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Dependencies;

namespace cCoder.DocumentManagement.Services.Processings;

internal sealed partial class FileContentEventProcessingService
{
    private static void ValidateInputs(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}