// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.DocumentManagement.Services.Processings;

public interface IDmsProcessingService
{
    ValueTask<DmsProcessingResponse> ProcessAsync(DmsProcessingRequest request);
}