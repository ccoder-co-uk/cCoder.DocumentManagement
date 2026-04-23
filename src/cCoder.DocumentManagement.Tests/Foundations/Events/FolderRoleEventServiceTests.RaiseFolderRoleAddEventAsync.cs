using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.Eventing.Models;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Foundations.Events;

public partial class FolderRoleEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaiseFolderRoleAddEventAsync()
    {
        // Given
        FolderRole entity = new() { FolderId = Guid.NewGuid(), RoleId = Guid.NewGuid() };
        EventMessage<cCoder.Data.Models.Security.FolderRole> actualMessage = null;

        folderRoleEventBrokerMock
            .Setup(
                x =>
                    x.RaiseFolderRoleAddEventAsync(
                        It.IsAny<EventMessage<cCoder.Data.Models.Security.FolderRole>>()
                    )
            )
            .Callback<EventMessage<cCoder.Data.Models.Security.FolderRole>>(message => actualMessage = message)
            .Returns(ValueTask.CompletedTask);

        // When
        await service.RaiseFolderRoleAddEventAsync(entity);

        // Then
        actualMessage.Should().NotBeNull();
        actualMessage!.Data.FolderId.Should().Be(entity.FolderId);
        actualMessage.Data.RoleId.Should().Be(entity.RoleId);
        actualMessage.AuthInfo.Should().NotBeNull();
        actualMessage.AuthInfo.SSOUserId.Should().Be(CurrentUserId);
        folderRoleEventBrokerMock.Verify(
            x =>
                x.RaiseFolderRoleAddEventAsync(
                    It.IsAny<EventMessage<cCoder.Data.Models.Security.FolderRole>>()
                ),
            Times.Once
        );
        folderRoleEventBrokerMock.VerifyNoOtherCalls();
    }

}








