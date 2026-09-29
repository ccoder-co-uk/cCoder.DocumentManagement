// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.DocumentManagement.Models;

namespace cCoder.DocumentManagement.Exposures;

internal interface IWebDavOperationsExposure
{
    ValueTask<DmsProcessingSession> ProcessDmsProcessingSessionAsync(DmsProcessingSession dmsProcessingSession);
}