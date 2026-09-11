// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.DocumentManagement.Services.Processings;

internal interface IDmsHttpProcessingService
{
    DmsHttpSession BuildDmsHttpSession(DmsHttpSession dmsHttpSession);
    ValueTask<DmsHttpSession> WriteDmsHttpSessionAsync(DmsHttpSession dmsHttpSession);
}