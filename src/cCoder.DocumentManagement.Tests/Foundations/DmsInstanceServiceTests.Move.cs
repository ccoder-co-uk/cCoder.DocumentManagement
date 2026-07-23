// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Moq;
using Xunit;
using DmsPath = cCoder.DocumentManagement.Models.Path;


namespace cCoder.Core.Services.Tests.DMS.Foundations;

public partial class DmsInstanceServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenMove()
    {
        // Given
        DmsPath oldPath = CreatePath(fullPath: "folder/old.txt");
        DmsPath newPath = CreatePath(fullPath: "folder/new.txt");
        dmsInstanceBrokerMock.Setup(expression: x => x.MoveAsync(oldPath, newPath)).Returns(value: ValueTask.CompletedTask);

        // When
        await dmsInstanceService.MoveAsync(oldPath: oldPath, newPath: newPath);

        // Then
        dmsInstanceBrokerMock.Verify(expression: x => x.MoveAsync(oldPath, newPath), times: Times.Once);
        dmsInstanceBrokerMock.VerifyNoOtherCalls();
    }

}