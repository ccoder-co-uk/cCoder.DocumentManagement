// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;


namespace cCoder.DocumentManagement.Services.Processings;

internal interface ICurrentAppResolverProcessingService
{
    App ResolveCurrentApp();
}