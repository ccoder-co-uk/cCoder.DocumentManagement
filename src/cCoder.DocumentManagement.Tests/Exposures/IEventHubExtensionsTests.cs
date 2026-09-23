// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Reflection;
using System.Text.Json;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.DocumentManagement.Models;
using cCoder.DocumentManagement.Services.Aggregations;
using cCoder.DocumentManagement.Services.Orchestrations;
using cCoder.Eventing;
using cCoder.Eventing.Models;
using FluentAssertions;
using Moq;
using Xunit;
using DataFolder = cCoder.Data.Models.DMS.Folder;
using DmsFile = cCoder.Data.Models.DMS.File;

namespace cCoder.DocumentManagement.Tests.Exposures;

public sealed partial class IEventHubExtensionsTests
{
    [Fact]
    public void DocumentManagementEvents_WhenRegisteredForWeb_UseTheEstablishedEventContract()
    {
        // Given
        Mock<IEventHub> eventHubMock = new(behavior: MockBehavior.Loose);

        // When
        InvokeEventRegistration(eventHub: eventHubMock.Object);

        // Then
        AssertExpectedRegistrations(eventHubMock: eventHubMock);
    }

    [Fact]
    public void DocumentManagementEvents_WhenRegisteredForHostedServices_UseTheEstablishedEventContract()
    {
        // Given
        Mock<IEventHub> eventHubMock = new(behavior: MockBehavior.Loose);

        // When
        InvokeEventRegistration(eventHub: eventHubMock.Object);

        // Then
        AssertExpectedRegistrations(eventHubMock: eventHubMock);
    }

    [Fact]
    public void DocumentManagementEventListening_WhenInspected_HasNoIntermediateListenerTypes()
    {
        // Given
        Assembly assembly = typeof(IServiceCollectionExtensions).Assembly;

        string[] listenerTypeNames =
        [
            "cCoder.DocumentManagement.Exposures.EventHandlers.IDocumentManagementEventHandlers",
            "cCoder.DocumentManagement.Exposures.EventHandlers.DocumentManagementEventHandlers",
            "cCoder.DocumentManagement.Services.Foundations.Events.IEventHandlerService",
            "cCoder.DocumentManagement.Services.Foundations.Events.EventHandlerService",
            "cCoder.DocumentManagement.Brokers.Events.IEventHubBroker",
            "cCoder.DocumentManagement.Brokers.Events.EventHubBroker",
        ];

        // When
        Type[] listenerTypes = listenerTypeNames
            .Select(selector: assembly.GetType)
            .ToArray();

        // Then
        listenerTypes.Should()
            .OnlyContain(predicate: listenerType => listenerType == null);
    }

    [Fact]
    public async Task PackageImportEvent_WhenHandled_PreservesTheExistingPackageMappingAsync()
    {
        // Given
        Mock<IEventHub> eventHubMock = new(behavior: MockBehavior.Loose);
        Mock<IDocumentManagementMigrationAggregationService> migrationServiceMock = new();
        Func<IDocumentManagementMigrationAggregationService, DocumentManagementPackageEvent, ValueTask> actualHandler = null;
        const int expectedAppId = 27;

        EventMessage<DocumentManagementPackageEvent> outboundMessage = new()
        {
            Data = new DocumentManagementPackageEvent
            {
                AppId = expectedAppId,
                Package = new Package { Name = "Documents" },
            },
        };

        eventHubMock.Setup(expression: eventHub => eventHub.ListenToEvent<
                DocumentManagementPackageEvent,
                IDocumentManagementMigrationAggregationService>(
                    name: "package_import",
                    handler: It.IsAny<Func<
                        IDocumentManagementMigrationAggregationService,
                        DocumentManagementPackageEvent,
                        ValueTask>>()))
            .Callback<string, Func<
                IDocumentManagementMigrationAggregationService,
                DocumentManagementPackageEvent,
                ValueTask>>(action: (_, handler) => actualHandler = handler);

        // When
        eventHubMock.Object.ListenToDocumentManagementEvents();
        string httpData = JsonSerializer.Serialize(value: outboundMessage.Data);

        DocumentManagementPackageEvent inboundEvent =
            JsonSerializer.Deserialize<DocumentManagementPackageEvent>(json: httpData);

        await actualHandler(
            arg1: migrationServiceMock.Object,
            arg2: inboundEvent);

        // Then
        migrationServiceMock.Verify(
            expression: service => service.ImportPackageDocumentManagementPackageAsync(
                appId: expectedAppId,
                documentManagementPackage: It.Is<DocumentManagementPackage>(match: package => package.Name == "Documents")),
            times: Times.Once);
    }

    private static void InvokeEventRegistration(IEventHub eventHub)
    {
        Type extensionsType = typeof(IServiceCollectionExtensions).Assembly.GetType(
            name: "cCoder.DocumentManagement.IEventHubExtensions");

        extensionsType.Should()
            .NotBeNull();

        MethodInfo listenMethod = extensionsType.GetMethod(
            name: "ListenToDocumentManagementEvents",
            bindingAttr: BindingFlags.Public | BindingFlags.Static);

        listenMethod.Should()
            .NotBeNull();

        listenMethod.Invoke(obj: null, parameters: [eventHub]);
    }

    private static void AssertExpectedRegistrations(Mock<IEventHub> eventHubMock)
    {
        eventHubMock.Verify(
            expression: eventHub => eventHub.ListenToEvent<App, IAppAggregationService>(
                name: "app_add",
                handler: It.IsAny<Func<IAppAggregationService, App, ValueTask>>()),
            times: Times.Once);

        eventHubMock.Verify(
            expression: eventHub => eventHub.ListenToEvent<App, IAppAggregationService>(
                name: "app_update",
                handler: It.IsAny<Func<IAppAggregationService, App, ValueTask>>()),
            times: Times.Once);

        eventHubMock.Verify(
            expression: eventHub => eventHub.ListenToEvent<App, IAppAggregationService>(
                name: "app_delete",
                handler: It.IsAny<Func<IAppAggregationService, App, ValueTask>>()),
            times: Times.Once);

        eventHubMock.Verify(
            expression: eventHub => eventHub.ListenToEvent<DataFolder, IFolderMutationAggregationService>(
                name: "folder_delete",
                handler: It.IsAny<Func<IFolderMutationAggregationService, DataFolder, ValueTask>>()),
            times: Times.Once);

        eventHubMock.Verify(
            expression: eventHub => eventHub.ListenToEvent<DmsFile, IFileMutationAggregationService>(
                name: "file_delete",
                handler: It.IsAny<Func<IFileMutationAggregationService, DmsFile, ValueTask>>()),
            times: Times.Once);

        eventHubMock.Verify(
            expression: eventHub => eventHub.ListenToEvent<DocumentManagementPackageEvent, IDocumentManagementMigrationAggregationService>(
                name: "package_import",
                handler: It.IsAny<Func<IDocumentManagementMigrationAggregationService, DocumentManagementPackageEvent, ValueTask>>()),
            times: Times.Once);

        eventHubMock.VerifyNoOtherCalls();
    }
}