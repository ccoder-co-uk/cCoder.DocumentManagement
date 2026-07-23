// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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

        appBrokerMock.Setup(expression: broker => broker.GetAppById(42)).Returns(value: dataApp);

        IDocumentManagementCurrentAppResolver resolver = new CurrentAppResolver(appBroker: appBrokerMock.Object, httpContext: httpContext);

        App result = resolver.ResolveCurrentApp();

        result.Should().BeEquivalentTo(expectation: CreateExpectedApp(dataApp));
        appBrokerMock.Verify(expression: broker => broker.GetAppById(42), times: Times.Once);
        appBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldThrowWhenWebDavCoreAppPathIdCannotBeResolved()
    {
        DefaultHttpContext httpContext = new();
        httpContext.Request.Path = "/webdav/Core/App(42)/Files";

        appBrokerMock.Setup(expression: broker => broker.GetAppById(42)).Returns(value: (DataApp)null);

        IDocumentManagementCurrentAppResolver resolver = new CurrentAppResolver(appBroker: appBrokerMock.Object, httpContext: httpContext);

        Action action = () => resolver.ResolveCurrentApp();

        action.Should().Throw<InvalidOperationException>()
            .WithMessage(expectedWildcardPattern: "Unable to resolve app '42'.");
        appBrokerMock.Verify(expression: broker => broker.GetAppById(42), times: Times.Once);
        appBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldResolveCurrentAppByHostWhenRequestIsNotWebDavCoreAppPath()
    {
        DataApp dataApp = CreateRandomDataApp(domain: "demo.localhost");
        DefaultHttpContext httpContext = new();
        httpContext.Request.Host = new HostString(value: "demo.localhost");
        httpContext.Request.Path = "/folder";

        appBrokerMock.Setup(expression: broker => broker.GetAppByDomain("demo.localhost")).Returns(value: dataApp);

        IDocumentManagementCurrentAppResolver resolver = new CurrentAppResolver(appBroker: appBrokerMock.Object, httpContext: httpContext);

        App result = resolver.ResolveCurrentApp();

        result.Should().BeEquivalentTo(expectation: CreateExpectedApp(dataApp));
        appBrokerMock.Verify(expression: broker => broker.GetAppByDomain("demo.localhost"), times: Times.Once);
        appBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldThrowWhenHostCannotBeResolved()
    {
        DefaultHttpContext httpContext = new();
        httpContext.Request.Host = new HostString(value: "missing.localhost");

        appBrokerMock.Setup(expression: broker => broker.GetAppByDomain("missing.localhost")).Returns(value: (DataApp)null);

        IDocumentManagementCurrentAppResolver resolver = new CurrentAppResolver(appBroker: appBrokerMock.Object, httpContext: httpContext);

        Action action = () => resolver.ResolveCurrentApp();

        action.Should().Throw<InvalidOperationException>()
            .WithMessage(expectedWildcardPattern: "Unable to resolve current app for host 'missing.localhost'.");
        appBrokerMock.Verify(expression: broker => broker.GetAppByDomain("missing.localhost"), times: Times.Once);
        appBrokerMock.VerifyNoOtherCalls();
    }
}