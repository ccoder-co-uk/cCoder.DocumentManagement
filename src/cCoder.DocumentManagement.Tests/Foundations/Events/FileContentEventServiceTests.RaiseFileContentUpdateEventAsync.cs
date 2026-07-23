// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.Eventing.Models;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Foundations.Events;

public partial class FileContentEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaiseFileContentUpdateEventAsync()
    {
        // Given
        FileContent entity = new() { Id = Guid.NewGuid(), FileId = Guid.NewGuid(), Version = 1 };
        EventMessage<cCoder.Data.Models.DMS.FileContent> actualMessage = null;

        fileContentEventBrokerMock
            .Setup(
                expression: x =>
                    x.RaiseFileContentUpdateEventAsync(
                        It.IsAny<EventMessage<cCoder.Data.Models.DMS.FileContent>>()
                    )
            )
            .Callback<EventMessage<cCoder.Data.Models.DMS.FileContent>>(action: message => actualMessage = message)
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseFileContentUpdateEventAsync(entity: entity);

        // Then
        actualMessage.Should().NotBeNull();
        actualMessage!.Data.Id.Should().Be(expected: entity.Id);
        actualMessage.Data.FileId.Should().Be(expected: entity.FileId);
        actualMessage.Data.Version.Should().Be(expected: entity.Version);
        actualMessage.AuthInfo.Should().NotBeNull();
        actualMessage.AuthInfo.SSOUserId.Should().Be(expected: CurrentUserId);
        fileContentEventBrokerMock.Verify(
            expression: x =>
                x.RaiseFileContentUpdateEventAsync(
                    It.IsAny<EventMessage<cCoder.Data.Models.DMS.FileContent>>()
                ),
            times: Times.Once
        );
        fileContentEventBrokerMock.VerifyNoOtherCalls();
    }

}