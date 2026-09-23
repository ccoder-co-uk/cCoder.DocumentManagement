// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Services.Aggregations;

namespace cCoder.DocumentManagement.Exposures;

internal sealed class WebDavOperationsExposure(
    IWebDavAggregationService webDavAggregationService)
    : IWebDavOperationsExposure
{
    public ValueTask<DmsProcessingSession> ProcessDmsProcessingSessionAsync(DmsProcessingSession dmsProcessingSession) =>
        webDavAggregationService.ProcessDmsProcessingSessionAsync(dmsProcessingSession: dmsProcessingSession);
}