// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Services.Aggregations;

namespace cCoder.DocumentManagement.Exposures;

internal sealed class DmsRequestOperationsExposure(
    IDmsRequestAggregationService dmsRequestAggregationService)
    : IDmsRequestOperationsExposure
{
    public ValueTask<DmsProcessingSession> ProcessDmsProcessingSessionAsync(DmsProcessingSession dmsProcessingSession) =>
        dmsRequestAggregationService.ProcessDmsProcessingSessionAsync(dmsProcessingSession: dmsProcessingSession);
}