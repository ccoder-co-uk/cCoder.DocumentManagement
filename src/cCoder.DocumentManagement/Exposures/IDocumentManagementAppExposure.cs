// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;

namespace cCoder.DocumentManagement.Exposures;

public interface IDocumentManagementAppExposure
{
    ValueTask AddAsync(App newApp);
    ValueTask UpdateAsync(App updatedApp);
    ValueTask DeleteAsync(int appId);
}