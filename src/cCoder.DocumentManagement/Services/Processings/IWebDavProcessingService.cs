// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.DocumentManagement.Services.Processings;

public interface IWebDavProcessingService
{
    ValueTask<DmsProcessingResponse> ProcessDmsProcessingRequestAsync(DmsProcessingRequest request);
}