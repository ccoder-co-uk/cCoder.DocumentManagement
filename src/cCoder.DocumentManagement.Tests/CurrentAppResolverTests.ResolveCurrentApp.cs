using cCoder.DocumentManagement.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;
using App = cCoder.Data.Models.CMS.App;
using DataApp = cCoder.Data.Models.CMS.App;


namespace cCoder.Core.Services.Tests.DMS;

public partial class CurrentAppResolverTests
{
    [Fact]
    public void ShouldResolveCurrentAppByIdForWebDavCoreAppPath()
    {
        DataApp dataApp = CreateRandomDataApp(id: 42, domain: "ignored.test");
        DefaultHttpContext httpContext = new();
        httpContext.Request.Path = "/webdav/Core/App(42)/Files";

        appBrokerMock.Setup(broker => broker.GetAppById(42)).Returns(dataApp);

        IDocumentManagementCurrentAppResolver resolver = new CurrentAppResolver(appBrokerMock.Object, httpContext);

        App result = resolver.ResolveCurrentApp();

        result.Should().BeEquivalentTo(CreateExpectedApp(dataApp));
        appBrokerMock.Verify(broker => broker.GetAppById(42), Times.Once);
        appBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldThrowWhenWebDavCoreAppPathIdCannotBeResolved()
    {
        DefaultHttpContext httpContext = new();
        httpContext.Request.Path = "/webdav/Core/App(42)/Files";

        appBrokerMock.Setup(broker => broker.GetAppById(42)).Returns((DataApp)null);

        IDocumentManagementCurrentAppResolver resolver = new CurrentAppResolver(appBrokerMock.Object, httpContext);

        Action action = () => resolver.ResolveCurrentApp();

        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Unable to resolve app '42'.");
        appBrokerMock.Verify(broker => broker.GetAppById(42), Times.Once);
        appBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldResolveCurrentAppByHostWhenRequestIsNotWebDavCoreAppPath()
    {
        DataApp dataApp = CreateRandomDataApp(domain: "demo.localhost");
        DefaultHttpContext httpContext = new();
        httpContext.Request.Host = new HostString("demo.localhost");
        httpContext.Request.Path = "/folder";

        appBrokerMock.Setup(broker => broker.GetAppByDomain("demo.localhost")).Returns(dataApp);

        IDocumentManagementCurrentAppResolver resolver = new CurrentAppResolver(appBrokerMock.Object, httpContext);

        App result = resolver.ResolveCurrentApp();

        result.Should().BeEquivalentTo(CreateExpectedApp(dataApp));
        appBrokerMock.Verify(broker => broker.GetAppByDomain("demo.localhost"), Times.Once);
        appBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldThrowWhenHostCannotBeResolved()
    {
        DefaultHttpContext httpContext = new();
        httpContext.Request.Host = new HostString("missing.localhost");

        appBrokerMock.Setup(broker => broker.GetAppByDomain("missing.localhost")).Returns((DataApp)null);

        IDocumentManagementCurrentAppResolver resolver = new CurrentAppResolver(appBrokerMock.Object, httpContext);

        Action action = () => resolver.ResolveCurrentApp();

        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Unable to resolve current app for host 'missing.localhost'.");
        appBrokerMock.Verify(broker => broker.GetAppByDomain("missing.localhost"), Times.Once);
        appBrokerMock.VerifyNoOtherCalls();
    }
}


