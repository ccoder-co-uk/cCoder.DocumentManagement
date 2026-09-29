// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;

namespace cCoder.DocumentManagement.Exposures;

public interface IDocumentManagementAppExposure
{
    ValueTask AddAsync(App newApp);
    ValueTask UpdateAsync(App updatedApp);
    ValueTask DeleteAsync(int appId);
}