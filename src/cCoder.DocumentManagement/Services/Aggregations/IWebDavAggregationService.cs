// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.DocumentManagement.Models;

namespace cCoder.DocumentManagement.Services.Aggregations;

internal interface IWebDavAggregationService
{
    ValueTask<DmsProcessingSession> ProcessDmsProcessingSessionAsync(DmsProcessingSession dmsProcessingSession);
}