// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Data;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Microsoft.EntityFrameworkCore;
using DataRole = cCoder.Data.Models.Security.Role;
using DataUser = cCoder.Data.Models.Security.User;
using DataUserRole = cCoder.Data.Models.Security.UserRole;
using LocalRole = cCoder.Data.Models.Security.Role;
using LocalUser = cCoder.Data.Models.Security.User;
using LocalUserRole = cCoder.Data.Models.Security.UserRole;


namespace cCoder.DocumentManagement.Brokers;

public interface IAuthorizationBroker
{
    LocalUser GetCurrentUser();
    bool IsAdminOfApp(int? appId);
    bool IsAdmin(int appId, string userName);
    void Authorize(int? appId, string privilege);
}

internal class AuthorizationBroker(ICoreContextFactory coreContextFactory) : IAuthorizationBroker
{
    public LocalUser GetCurrentUser()
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        return ToLocalUser(user: coreDataContext.User);
    }

    public bool IsAdminOfApp(int? appId)
    {
        LocalUser user = GetCurrentUser();
        return user != null && appId.HasValue && HasAppAdminPrivilege(user: user, appId: appId.Value);
    }

    public bool IsAdmin(int appId, string userName)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        DataUser user = coreDataContext.Users
            .Include(navigationPropertyPath: foundUser => foundUser.Roles)
            .FirstOrDefault(predicate: foundUser => foundUser.Id == userName);

        App app = coreDataContext.Apps
            .Include(navigationPropertyPath: foundApp => foundApp.Roles.Select(role => role.Users))
            .FirstOrDefault(predicate: foundApp => foundApp.Id == appId);

        return app?.IsAppAdmin(user: user) ?? false;
    }

    public void Authorize(int? appId, string privilege)
    {
        LocalUser user = GetCurrentUser();

        if (user == null || !(HasAppAdminPrivilege(user: user, appId: appId) || HasPrivilege(user: user, appId: appId, privilege: privilege)))
        {
            throw new SecurityException(message: "Access Denied!");
        }
    }

    private static bool HasPrivilege(LocalUser user, int? appId, string privilege)
    {
        string normalizedPrivilege = privilege.ToLower();

        return (appId != null && HasAppAdminPrivilege(user: user, appId: appId.Value))
            || (user.Roles?.Any(predicate: role =>
                (appId == null || role.Role.AppId == appId)
                && role.Role.Privileges.Contains(normalizedPrivilege))
                ?? false);
    }

    private static bool HasAppAdminPrivilege(LocalUser user, int? appId) =>
        appId.HasValue
        && (user.Roles?.Any(predicate: role => role.Role.AppId == appId.Value && role.Role.Allows(user, "app_admin")) ?? false);

    private static LocalUser ToLocalUser(DataUser user)
    {
        if (user == null)
        {
            return null;
        }

        return new LocalUser
        {
            Id = user.Id,
            DefaultCultureId = user.DefaultCultureId,
            DisplayName = user.DisplayName,
            Email = user.Email,
            IsActive = user.IsActive,
            DefaultCulture = user.DefaultCulture,
            Roles = user.Roles?.Select(selector: ToLocalUserRole).ToList(),
        };
    }

    private static LocalUserRole ToLocalUserRole(DataUserRole userRole)
    {
        if (userRole == null)
        {
            return null;
        }

        return new LocalUserRole
        {
            UserId = userRole.UserId,
            RoleId = userRole.RoleId,
            Role = ToLocalRole(role: userRole.Role),
        };
    }

    private static LocalRole ToLocalRole(DataRole role)
    {
        if (role == null)
        {
            return null;
        }

        return new LocalRole
        {
            Id = role.Id,
            AppId = role.AppId,
            Name = role.Name,
            Description = role.Description,
            Privs = role.Privs,
        };
    }
}