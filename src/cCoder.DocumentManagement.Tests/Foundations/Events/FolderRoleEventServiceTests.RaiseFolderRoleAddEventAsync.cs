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
                expression: x =>
                    x.RaiseFolderRoleAddEventAsync(
                        It.IsAny<EventMessage<cCoder.Data.Models.Security.FolderRole>>()
                    )
            )
            .Callback<EventMessage<cCoder.Data.Models.Security.FolderRole>>(action: message => actualMessage = message)
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseFolderRoleAddEventAsync(entity: entity);

        // Then
        actualMessage.Should().NotBeNull();
        actualMessage!.Data.FolderId.Should().Be(expected: entity.FolderId);
        actualMessage.Data.RoleId.Should().Be(expected: entity.RoleId);
        actualMessage.AuthInfo.Should().NotBeNull();
        actualMessage.AuthInfo.SSOUserId.Should().Be(expected: CurrentUserId);
        folderRoleEventBrokerMock.Verify(
            expression: x =>
                x.RaiseFolderRoleAddEventAsync(
                    It.IsAny<EventMessage<cCoder.Data.Models.Security.FolderRole>>()
                ),
            times: Times.Once
        );
        folderRoleEventBrokerMock.VerifyNoOtherCalls();
    }

}