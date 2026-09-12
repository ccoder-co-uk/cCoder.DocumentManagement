// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.DocumentManagement.Services.Foundations;

internal interface IDmsHttpService
{
    DmsHttpSession BuildDmsHttpSession(DmsHttpSession dmsHttpSession);
    ValueTask<DmsHttpSession> WriteDmsHttpSessionAsync(DmsHttpSession dmsHttpSession);
}