using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using EventLibrary.Models;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Foundations.Events;

public partial class FolderEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaiseFolderUpdateEventAsync()
    {
        // Given
        Folder entity = new() { Id = Guid.NewGuid(), AppId = 1, Name = "root", Path = "root" };
        EventMessage<cCoder.Data.Models.DMS.Folder> actualMessage = null;

        folderEventBrokerMock
            .Setup(
                x =>
                    x.RaiseFolderUpdateEventAsync(
                        It.IsAny<EventMessage<cCoder.Data.Models.DMS.Folder>>()
                    )
            )
            .Callback<EventMessage<cCoder.Data.Models.DMS.Folder>>(message => actualMessage = message)
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseFolderUpdateEventAsync(entity);

        // Then
        actualMessage.Should().NotBeNull();
        actualMessage!.Data.Id.Should().Be(entity.Id);
        actualMessage.Data.AppId.Should().Be(entity.AppId);
        actualMessage.Data.Name.Should().Be(entity.Name);
        actualMessage.Data.Path.Should().Be(entity.Path);
        actualMessage.AuthInfo.Should().NotBeNull();
        actualMessage.AuthInfo.SSOUserId.Should().Be(CurrentUserId);
        folderEventBrokerMock.Verify(
            x =>
                x.RaiseFolderUpdateEventAsync(
                    It.IsAny<EventMessage<cCoder.Data.Models.DMS.Folder>>()
                ),
            Times.Once
        );
        folderEventBrokerMock.VerifyNoOtherCalls();
    }

}








