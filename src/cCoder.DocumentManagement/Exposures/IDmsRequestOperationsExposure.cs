// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.DocumentManagement.Exposures;

internal interface IDmsRequestOperationsExposure
{
    ValueTask<DmsProcessingSession> ProcessDmsProcessingSessionAsync(DmsProcessingSession dmsProcessingSession);
}