// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;

namespace cCoder.DocumentManagement.Services.Aggregations;

internal interface IAppAggregationService
{
    ValueTask AddAppAsync(App newApp);
    ValueTask UpdateAppAsync(App updatedApp);
    ValueTask DeleteAsync(int appId);
}