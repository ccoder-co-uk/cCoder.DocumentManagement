// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;

namespace cCoder.DocumentManagement.Extensions;

internal static class RoleExtensions
{
    internal static Role ToLocalRole(this Role role) =>
        role is null
            ? null
            : new Role
            {
                Id = role.Id,
                AppId = role.AppId,
                Name = role.Name,
                Description = role.Description,
                Privs = role.Privs
            };
}