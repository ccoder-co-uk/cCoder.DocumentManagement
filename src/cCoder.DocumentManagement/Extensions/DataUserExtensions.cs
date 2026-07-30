// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using DataUser = cCoder.Data.Models.Security.User;
using DataUserRole = cCoder.Data.Models.Security.UserRole;
using LocalUser = cCoder.Data.Models.Security.User;
using LocalUserRole = cCoder.Data.Models.Security.UserRole;

namespace cCoder.DocumentManagement.Extensions;

internal static class DataUserExtensions
{
    internal static LocalUser ToLocalUser(this DataUser user) =>
        user is null
            ? null
            : new LocalUser
            {
                Id = user.Id,
                DefaultCultureId = user.DefaultCultureId,
                DisplayName = user.DisplayName,
                Email = user.Email,
                IsActive = user.IsActive,
                DefaultCulture = user.DefaultCulture,
                Roles = user.Roles?
                    .Select(selector: ToLocalUserRole)
                    .ToList()
            };

    private static LocalUserRole ToLocalUserRole(DataUserRole userRole) =>
        userRole is null
            ? null
            : new LocalUserRole
            {
                UserId = userRole.UserId,
                RoleId = userRole.RoleId,
                Role = userRole.Role.ToLocalRole()
            };
}