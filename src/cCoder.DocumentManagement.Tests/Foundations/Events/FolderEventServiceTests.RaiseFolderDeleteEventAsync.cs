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

public partial class FolderEventServiceTests
{
    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaiseFolderDeleteEventAsync()
    {
        // Given
        Folder entity = new() { Id = Guid.NewGuid(), AppId = 1, Name = "root", Path = "root" };
        EventMessage<cCoder.Data.Models.DMS.Folder> actualMessage = null;

        folderEventBrokerMock
            .Setup(
                expression: x =>
                    x.RaiseFolderDeleteEventAsync(
                        message: It.IsAny<EventMessage<cCoder.Data.Models.DMS.Folder>>()
                    )
            )
            .Callback<EventMessage<cCoder.Data.Models.DMS.Folder>>(action: message => actualMessage = message)
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseFolderDeleteEventAsync(entity: entity);

        // Then
        actualMessage.Should()
            .NotBeNull();

        actualMessage!.Data.Id.Should()
            .Be(expected: entity.Id);

        actualMessage.Data.AppId.Should()
            .Be(expected: entity.AppId);

        actualMessage.Data.Name.Should()
            .Be(expected: entity.Name);

        actualMessage.Data.Path.Should()
            .Be(expected: entity.Path);

        actualMessage.AuthInfo.Should()
            .NotBeNull();

        actualMessage.AuthInfo.SSOUserId.Should()
            .Be(expected: CurrentUserId);

        folderEventBrokerMock.Verify(
            expression: x =>
                x.RaiseFolderDeleteEventAsync(
                    message: It.IsAny<EventMessage<cCoder.Data.Models.DMS.Folder>>()
                ),
            times: Times.Once
        );

        folderEventBrokerMock.VerifyNoOtherCalls();
    }

}