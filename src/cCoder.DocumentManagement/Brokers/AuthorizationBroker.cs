// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Dependencies;
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
        return coreDataContext.User.ToLocalUser();
    }

    public bool IsAdminOfApp(int? appId) =>
        GetCurrentUser()
            .IsAdminOfApp(appId: appId);

    public bool IsAdmin(int appId, string userName)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        DataUser user = coreDataContext.Users
            .Include(navigationPropertyPath: foundUser => foundUser.Roles)
            .FirstOrDefault(predicate: foundUser => foundUser.Id == userName);

        App app = coreDataContext.Apps
            .Include(navigationPropertyPath: foundApp => foundApp.Roles.Select(selector: role => role.Users))
            .FirstOrDefault(predicate: foundApp => foundApp.Id == appId);

        return app?.IsAppAdmin(user: user) ?? false;
    }

    public void Authorize(int? appId, string privilege) =>
        GetCurrentUser()
            .ThrowIfUnauthorized(
                appId: appId,
                privilege: privilege);
}