// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;
using FluentAssertions;
using Moq;
using Xunit;
using DataFile = cCoder.Data.Models.DMS.File;
using LocalFile = cCoder.Data.Models.DMS.File;


namespace cCoder.Core.Services.Tests.DMS.Foundations.Events;

public partial class FileEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaiseFileDeleteEventAsync()
    {
        // Given
        LocalFile entity = new() { Id = Guid.NewGuid(), Name = "file.txt", Path = "file.txt" };
        EventMessage<DataFile> actualMessage = null;

        fileEventBrokerMock
            .Setup(expression: x => x.RaiseFileDeleteEventAsync(It.IsAny<EventMessage<DataFile>>()))
            .Callback<EventMessage<DataFile>>(action: message => actualMessage = message)
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseFileDeleteEventAsync(entity: entity);

        // Then
        actualMessage.Should().NotBeNull();
        actualMessage!.Data.Id.Should().Be(expected: entity.Id);
        actualMessage.Data.Name.Should().Be(expected: entity.Name);
        actualMessage.Data.Path.Should().Be(expected: entity.Path);
        actualMessage.AuthInfo.Should().NotBeNull();
        actualMessage.AuthInfo.SSOUserId.Should().Be(expected: CurrentUserId);
        fileEventBrokerMock.Verify(
            expression: x => x.RaiseFileDeleteEventAsync(It.IsAny<EventMessage<DataFile>>()),
            times: Times.Once
        );
        fileEventBrokerMock.VerifyNoOtherCalls();
    }

}