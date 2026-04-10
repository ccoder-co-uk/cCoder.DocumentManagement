using cCoder.DocumentManagement.Brokers.Storage;
using FizzWare.NBuilder;
using Moq;
using App = cCoder.Data.Models.CMS.App;
using DataApp = cCoder.Data.Models.CMS.App;


namespace cCoder.Core.Services.Tests.DMS;

public partial class CurrentAppResolverTests
{
    private readonly Mock<IAppBroker> appBrokerMock;

    public CurrentAppResolverTests()
    {
        appBrokerMock = new Mock<IAppBroker>(MockBehavior.Strict);
    }

    private static DataApp CreateRandomDataApp(int id = 0, string domain = null) =>
        Builder<DataApp>
            .CreateNew()
            .With(app => app.Id = id == 0 ? Random.Shared.Next(1, 1000) : id)
            .With(app => app.Domain = domain ?? $"app-{Guid.NewGuid():N}.test")
            .With(app => app.ConfigJson = "{}")
            .Build();

    private static App CreateExpectedApp(DataApp app) =>
        Builder<App>
            .CreateNew()
            .With(localApp => localApp.Id = app.Id)
            .With(localApp => localApp.DefaultCultureId = app.DefaultCultureId)
            .With(localApp => localApp.TenantId = app.TenantId)
            .With(localApp => localApp.Name = app.Name)
            .With(localApp => localApp.Domain = app.Domain)
            .With(localApp => localApp.DefaultTheme = app.DefaultTheme)
            .With(localApp => localApp.ConfigJson = app.ConfigJson)
            .With(localApp => localApp.Roles = [])
            .With(localApp => localApp.Folders = [])
            .Build();
}



