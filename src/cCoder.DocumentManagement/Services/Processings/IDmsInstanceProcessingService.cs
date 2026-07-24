// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.DocumentManagement.Services.Processings;

public interface IDmsInstanceProcessingService
{
    ValueTask<DmsProcessingResponse> ProcessDmsProcessingRequestAsync(DmsProcessingRequest request);
}
