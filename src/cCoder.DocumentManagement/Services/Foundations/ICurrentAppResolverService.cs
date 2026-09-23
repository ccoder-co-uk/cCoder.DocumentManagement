// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.DocumentManagement.Services.Foundations;

internal interface ICurrentAppResolverService
{
    App ResolveCurrentApp();
}