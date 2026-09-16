// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.DocumentManagement.Services.Foundations;

internal sealed partial class PackagePayloadShapeService
    : IPackagePayloadShapeService
{
    public bool IsSingleItem(string data) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [data]);

            return data.StartsWith(value: "{");
        });
}