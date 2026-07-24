// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using App = cCoder.Data.Models.CMS.App;


namespace cCoder.DocumentManagement.Brokers.Storage;

public interface IAppBroker
{
    App SelectAppById(int appId);
    App SelectAppByDomain(string domain);
}

internal sealed class AppBroker(ICoreContextFactory coreContextFactory) : IAppBroker
{
    public App SelectAppById(int appId)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.Apps.FirstOrDefault(predicate: app => app.Id == appId);
    }

    public App SelectAppByDomain(string domain)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.Apps.FirstOrDefault(predicate: app => app.Domain == domain);
    }
}