// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.DocumentManagement.Services.Foundations;

internal sealed partial class PackagePayloadShapeService
{
    private static void ValidateInputs(params object[] inputs) =>
        Dependencies.ValidationRulesEngine.Validate(inputs: inputs);
}