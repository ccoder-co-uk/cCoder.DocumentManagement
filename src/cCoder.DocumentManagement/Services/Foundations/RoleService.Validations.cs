// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.DocumentManagement.Services.Foundations;

internal sealed partial class RoleService
{
    private static void ValidateInputs(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllOnGet(bool ignoreFilters) =>
        Validate(inputs: [ignoreFilters]);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}