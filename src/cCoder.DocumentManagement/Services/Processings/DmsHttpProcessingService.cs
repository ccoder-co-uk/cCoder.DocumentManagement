// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Services.Foundations;

namespace cCoder.DocumentManagement.Services.Processings;

internal sealed partial class DmsHttpProcessingService(
    IDmsHttpService dmsHttpService)
    : IDmsHttpProcessingService
{
    public DmsHttpSession BuildDmsHttpSession(DmsHttpSession dmsHttpSession) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [dmsHttpSession]);

            return dmsHttpService.BuildDmsHttpSession(
                dmsHttpSession: dmsHttpSession);
        });

    public ValueTask<DmsHttpSession> WriteDmsHttpSessionAsync(
        DmsHttpSession dmsHttpSession) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [dmsHttpSession]);

            return await dmsHttpService.WriteDmsHttpSessionAsync(
                dmsHttpSession: dmsHttpSession);
        });
}