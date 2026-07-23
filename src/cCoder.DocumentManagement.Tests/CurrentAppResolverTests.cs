// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        appBrokerMock = new Mock<IAppBroker>(behavior: MockBehavior.Strict);
    }

    private static DataApp CreateRandomDataApp(int id = 0, string domain = null) =>
        Builder<DataApp>
            .CreateNew()
            .With(func: app => app.Id = id == 0 ? Random.Shared.Next(1, 1000) : id)
            .With(func: app => app.Domain = domain ?? $"app-{Guid.NewGuid():N}.test")
            .With(func: app => app.ConfigJson = "{}")
            .Build();

    private static App CreateExpectedApp(DataApp app) =>
        Builder<App>
            .CreateNew()
            .With(func: localApp => localApp.Id = app.Id)
            .With(func: localApp => localApp.DefaultCultureId = app.DefaultCultureId)
            .With(func: localApp => localApp.TenantId = app.TenantId)
            .With(func: localApp => localApp.Name = app.Name)
            .With(func: localApp => localApp.Domain = app.Domain)
            .With(func: localApp => localApp.DefaultTheme = app.DefaultTheme)
            .With(func: localApp => localApp.ConfigJson = app.ConfigJson)
            .With(func: localApp => localApp.Roles = [])
            .With(func: localApp => localApp.Folders = [])
            .Build();
}