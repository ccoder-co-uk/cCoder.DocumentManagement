// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;

namespace cCoder.DocumentManagement.Services.Orchestrations;

public interface IAppOrchestrationService
{
    ValueTask AddAppAsync(App app);
    ValueTask UpdateAppAsync(App app);
    ValueTask DeleteAsync(int appId);
}