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
    public async Task ShouldMapAndCallBrokerWhenRaiseFileUpdateEventAsync()
    {
        // Given
        LocalFile entity = new() { Id = Guid.NewGuid(), Name = "file.txt", Path = "file.txt" };
        EventMessage<DataFile> actualMessage = null;

        fileEventBrokerMock
            .Setup(x => x.RaiseFileUpdateEventAsync(It.IsAny<EventMessage<DataFile>>()))
            .Callback<EventMessage<DataFile>>(message => actualMessage = message)
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseFileUpdateEventAsync(entity);

        // Then
        actualMessage.Should().NotBeNull();
        actualMessage!.Data.Id.Should().Be(entity.Id);
        actualMessage.Data.Name.Should().Be(entity.Name);
        actualMessage.Data.Path.Should().Be(entity.Path);
        actualMessage.AuthInfo.Should().NotBeNull();
        actualMessage.AuthInfo.SSOUserId.Should().Be(CurrentUserId);
        fileEventBrokerMock.Verify(
            x => x.RaiseFileUpdateEventAsync(It.IsAny<EventMessage<DataFile>>()),
            Times.Once
        );
        fileEventBrokerMock.VerifyNoOtherCalls();
    }

}








