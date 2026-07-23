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
    public async Task ShouldDelegateToBrokerWhenDrop()
    {
        // Given
        DmsPath path = CreatePath(fullPath: "folder/file.txt");

        dmsInstanceBrokerMock.Setup(expression: x => x.DropAsync(path: path, version: 7))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await dmsInstanceService.DropAsync(path: path, version: 7);

        // Then
        dmsInstanceBrokerMock.Verify(expression: x => x.DropAsync(path: path, version: 7), times: Times.Once);
        dmsInstanceBrokerMock.VerifyNoOtherCalls();
    }

}