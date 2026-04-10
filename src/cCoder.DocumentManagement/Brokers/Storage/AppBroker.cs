using cCoder.Data;
using App = cCoder.Data.Models.CMS.App;


namespace cCoder.DocumentManagement.Brokers.Storage;

public interface IAppBroker
{
    App GetAppById(int appId);
    App GetAppByDomain(string domain);
}

internal sealed class AppBroker(ICoreContextFactory coreContextFactory) : IAppBroker
{
    public App GetAppById(int appId)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.Apps.FirstOrDefault(app => app.Id == appId);
    }

    public App GetAppByDomain(string domain)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.Apps.FirstOrDefault(app => app.Domain == domain);
    }
}


