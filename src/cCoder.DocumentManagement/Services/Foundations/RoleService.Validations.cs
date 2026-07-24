// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Dependencies;

namespace cCoder.DocumentManagement.Services.Foundations;

internal sealed partial class RoleService
{
    private static void ValidateInputs(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}