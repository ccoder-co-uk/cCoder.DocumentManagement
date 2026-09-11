// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.DocumentManagement.Services.Processings;

internal sealed partial class DmsHttpProcessingService
{
    private static void ValidateInputs(params object[] inputs)
    {
        if (inputs.Any(predicate: input => input is null))
        {
            throw new ArgumentNullException(paramName: nameof(inputs));
        }
    }
}