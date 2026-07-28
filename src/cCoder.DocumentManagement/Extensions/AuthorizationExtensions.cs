// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Data.Models.Security;
using DataRole = cCoder.Data.Models.Security.Role;
using DataUser = cCoder.Data.Models.Security.User;
using DataUserRole = cCoder.Data.Models.Security.UserRole;

namespace cCoder.DocumentManagement.Extensions;

internal static class AuthorizationExtensions
{
    internal static void ThrowIfUnauthorized(
        this User user,
        int? appId,
        string privilege)
    {
        if (user is null
            || !(user.IsAdminOfApp(appId: appId)
                || user.HasPrivilege(
                    appId: appId,
                    privilege: privilege)))
        {
            throw new SecurityException(message: "Access Denied!");
        }
    }

    internal static User ToLocalUser(this DataUser user) =>
        user is null
            ? null
            : new User
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

    private static bool HasPrivilege(
        this User user,
        int? appId,
        string privilege)
    {
        string normalizedPrivilege = privilege.ToLower();

        return (appId is not null
                && user.IsAdminOfApp(appId: appId.Value))
            || (user.Roles?.Any(predicate: role =>
                (appId is null || role.Role.AppId == appId)
                && role.Role.Privileges.Contains(item: normalizedPrivilege))
                ?? false);
    }

    private static UserRole ToLocalUserRole(DataUserRole userRole) =>
        userRole is null
            ? null
            : new UserRole
            {
                UserId = userRole.UserId,
                RoleId = userRole.RoleId,
                Role = userRole.Role.ToLocalRole()
            };

    private static Role ToLocalRole(this DataRole role) =>
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