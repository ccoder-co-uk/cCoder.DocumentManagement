// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.DocumentManagement.Services.Processings;

internal sealed partial class RoleMigrationRetrievalProcessingService
{
    private static void ValidateInputs(params object[] inputs) =>
        Dependencies.ValidationRulesEngine.Validate(inputs: inputs);
}