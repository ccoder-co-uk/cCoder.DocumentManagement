// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.DocumentManagement.Services.Processings;

internal sealed partial class PackagePayloadShapeProcessingService
    : IPackagePayloadShapeProcessingService
{
    public bool IsSingleItem(string data) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [data]);

            return data.StartsWith(value: "{");
        });
}