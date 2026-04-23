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
    public async Task ShouldMapAndCallBrokerWhenRaiseFileContentAddEventAsync()
    {
        // Given
        FileContent entity = new() { Id = Guid.NewGuid(), FileId = Guid.NewGuid(), Version = 1 };
        EventMessage<cCoder.Data.Models.DMS.FileContent> actualMessage = null;

        fileContentEventBrokerMock
            .Setup(
                x =>
                    x.RaiseFileContentAddEventAsync(
                        It.IsAny<EventMessage<cCoder.Data.Models.DMS.FileContent>>()
                    )
            )
            .Callback<EventMessage<cCoder.Data.Models.DMS.FileContent>>(message => actualMessage = message)
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseFileContentAddEventAsync(entity);

        // Then
        actualMessage.Should().NotBeNull();
        actualMessage!.Data.Id.Should().Be(entity.Id);
        actualMessage.Data.FileId.Should().Be(entity.FileId);
        actualMessage.Data.Version.Should().Be(entity.Version);
        actualMessage.AuthInfo.Should().NotBeNull();
        actualMessage.AuthInfo.SSOUserId.Should().Be(CurrentUserId);
        fileContentEventBrokerMock.Verify(
            x =>
                x.RaiseFileContentAddEventAsync(
                    It.IsAny<EventMessage<cCoder.Data.Models.DMS.FileContent>>()
                ),
            Times.Once
        );
        fileContentEventBrokerMock.VerifyNoOtherCalls();
    }

}








