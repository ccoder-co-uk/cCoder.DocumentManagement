using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using EventLibrary.Models;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Foundations.Events;

public partial class FolderRoleEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaiseFolderRoleDeleteEventAsync()
    {
        // Given
        FolderRole entity = new() { FolderId = Guid.NewGuid(), RoleId = Guid.NewGuid() };
        EventMessage<cCoder.Data.Models.Security.FolderRole> actualMessage = null;

        folderRoleEventBrokerMock
            .Setup(
                x =>
                    x.RaiseFolderRoleDeleteEventAsync(
                        It.IsAny<EventMessage<cCoder.Data.Models.Security.FolderRole>>()
                    )
            )
            .Callback<EventMessage<cCoder.Data.Models.Security.FolderRole>>(message => actualMessage = message)
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseFolderRoleDeleteEventAsync(entity);

        // Then
        actualMessage.Should().NotBeNull();
        actualMessage!.Data.FolderId.Should().Be(entity.FolderId);
        actualMessage.Data.RoleId.Should().Be(entity.RoleId);
        actualMessage.AuthInfo.Should().NotBeNull();
        actualMessage.AuthInfo.SSOUserId.Should().Be(CurrentUserId);
        folderRoleEventBrokerMock.Verify(
            x =>
                x.RaiseFolderRoleDeleteEventAsync(
                    It.IsAny<EventMessage<cCoder.Data.Models.Security.FolderRole>>()
                ),
            Times.Once
        );
        folderRoleEventBrokerMock.VerifyNoOtherCalls();
    }

}








