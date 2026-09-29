// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.DocumentManagement.Models;

namespace cCoder.DocumentManagement.Services.Foundations;

internal interface IDmsHttpService
{
    DmsHttpSession BuildDmsHttpSession(DmsHttpSession dmsHttpSession);
    ValueTask<DmsHttpSession> WriteDmsHttpSessionAsync(DmsHttpSession dmsHttpSession);
}