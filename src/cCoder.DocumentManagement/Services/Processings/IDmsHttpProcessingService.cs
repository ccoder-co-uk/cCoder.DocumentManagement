// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.DocumentManagement.Models;

namespace cCoder.DocumentManagement.Services.Processings;

internal interface IDmsHttpProcessingService
{
    DmsHttpSession BuildDmsHttpSession(DmsHttpSession dmsHttpSession);
    ValueTask<DmsHttpSession> WriteDmsHttpSessionAsync(DmsHttpSession dmsHttpSession);
}