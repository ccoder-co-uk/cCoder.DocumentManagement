// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


namespace cCoder.DocumentManagement.Services.Aggregations;

internal sealed partial class AppAggregationService
{
    private static void ValidateInputs(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAppOnAdd(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAppOnUpdate(params object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}