// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;

namespace cCoder.DocumentManagement.Services.Processings;

internal interface IRoleMigrationRetrievalProcessingService
{
    IQueryable<Role> GetAllRoles(bool ignoreFilters);
}