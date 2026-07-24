// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Orchestrations;

namespace cCoder.DocumentManagement.Exposures;

internal class DocumentManagementAppExposure(IAppOrchestrationService appOrchestrationService)
    : IDocumentManagementAppExposure
{
    public ValueTask AddAsync(App newApp) =>
        appOrchestrationService.AddAppAsync(newApp: newApp);
    public ValueTask UpdateAsync(App updatedApp) =>
        appOrchestrationService.UpdateAppAsync(updatedApp: updatedApp);
    public ValueTask DeleteAsync(int appId) =>
        appOrchestrationService.DeleteAsync(appId: appId);
}