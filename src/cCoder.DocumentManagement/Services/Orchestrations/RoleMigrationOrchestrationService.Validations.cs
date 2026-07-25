// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.DocumentManagement.Services.Orchestrations;

internal sealed partial class RoleMigrationOrchestrationService
{
    private static void ValidateInputs(params object[] inputs) =>
        Dependencies.ValidationRulesEngine.Validate(inputs: inputs);
}