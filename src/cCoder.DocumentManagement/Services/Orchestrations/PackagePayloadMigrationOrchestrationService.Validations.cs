// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.DocumentManagement.Services.Orchestrations;

internal sealed partial class PackagePayloadMigrationOrchestrationService
{
    private static void ValidateInputs(params object[] inputs) =>
        Dependencies.ValidationRulesEngine.Validate(inputs: inputs);
}