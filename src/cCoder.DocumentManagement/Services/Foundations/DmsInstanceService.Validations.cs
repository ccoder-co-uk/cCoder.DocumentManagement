// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Dependencies;
using DmsPath = cCoder.DocumentManagement.Models.Path;

namespace cCoder.DocumentManagement.Services.Foundations;

internal sealed partial class DmsInstanceService
{
    private static void ValidateInputs(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateFilesZippedOnGet(IEnumerable<DmsPath> paths) =>
        ValidationRulesEngine.Validate(inputs: [paths]);
}