// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;
using cCoder.DocumentManagement.Brokers.Events;
using cCoder.DocumentManagement.Models;
using cCoder.DocumentManagement.Services.Aggregations;
using cCoder.DocumentManagement.Services.Foundations.Events;
using cCoder.Eventing.Models;
using Moq;
using System.Text.Json;
using Xunit;

namespace cCoder.DocumentManagement.Tests.Foundations.Events;

public sealed partial class EventHandlerServiceTests
{
    [Fact]
    public async Task ShouldHandleSerializedPackageImportEventAsync()
    {
        // Given
        Mock<IEventHubBroker> eventHubBrokerMock = new(behavior: MockBehavior.Loose);
        Mock<IDocumentManagementMigrationAggregationService> migrationServiceMock = new();
        Func<IDocumentManagementMigrationAggregationService, DocumentManagementPackageEvent, ValueTask> actualHandler = null;
        const int expectedAppId = 27;

        EventMessage<DocumentManagementPackageEvent> outboundMessage = new()
        {
            Data = new DocumentManagementPackageEvent
            {
                AppId = expectedAppId,
                Package = new Package { Name = "Documents" }
            }
        };

        eventHubBrokerMock.Setup(expression: broker => broker.ListenToEvent<
                DocumentManagementPackageEvent,
                IDocumentManagementMigrationAggregationService>(
                    eventName: "package_import",
                    handler: It.IsAny<Func<
                        IDocumentManagementMigrationAggregationService,
                        DocumentManagementPackageEvent,
                        ValueTask>>()))
            .Callback<string, Func<
                IDocumentManagementMigrationAggregationService,
                DocumentManagementPackageEvent,
                ValueTask>>(action: (_, handler) => actualHandler = handler);

        EventHandlerService eventHandlerService = new(
            eventHubBroker: eventHubBrokerMock.Object);

        // When
        eventHandlerService.ListenToAllEvents();
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
}