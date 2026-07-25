// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;

namespace cCoder.DocumentManagement.Services.Processings;

internal interface IRoleMigrationFilterProcessingService
{
    Role[] FilterRolesForApp(
        IEnumerable<Role> roles,
        int appId);
}