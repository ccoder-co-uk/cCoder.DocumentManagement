// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.DocumentManagement.Exposures;

internal interface IWebDavOperationsExposure
{
    ValueTask<DmsProcessingSession> ProcessDmsProcessingSessionAsync(DmsProcessingSession dmsProcessingSession);
}