// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Services.Aggregations;

namespace cCoder.DocumentManagement.Exposures;

internal sealed class DmsRequestOperationsExposure(
    IDmsRequestAggregationService dmsRequestAggregationService)
    : IDmsRequestOperationsExposure,
      cCoder.CodeAnalysis.Exposures.ICompositionExposure
{
    public ValueTask<DmsProcessingSession> ProcessDmsProcessingSessionAsync(DmsProcessingSession dmsProcessingSession) =>
        dmsRequestAggregationService.ProcessDmsProcessingSessionAsync(dmsProcessingSession: dmsProcessingSession);
}