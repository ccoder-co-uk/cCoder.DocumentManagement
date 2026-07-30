// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Data.Models.Security;

namespace cCoder.DocumentManagement.Extensions;

internal static class UserExtensions
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

}