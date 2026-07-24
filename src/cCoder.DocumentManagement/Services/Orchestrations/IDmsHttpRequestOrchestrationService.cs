// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Services.Processings;


namespace cCoder.DocumentManagement.Services.Orchestrations;

public interface IDmsHttpRequestOrchestrationService
{
    ValueTask ProcessRequestAsync(HttpContext context);
}
