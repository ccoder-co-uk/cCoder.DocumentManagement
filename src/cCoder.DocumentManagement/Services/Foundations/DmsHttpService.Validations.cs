// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.DocumentManagement.Services.Foundations;

internal sealed partial class DmsHttpService
{
    private static void ValidateInputs(params object[] inputs)
    {
        if (inputs.Any(predicate: input => input is null))
        {
            throw new ArgumentNullException(paramName: nameof(inputs));
        }
    }
}