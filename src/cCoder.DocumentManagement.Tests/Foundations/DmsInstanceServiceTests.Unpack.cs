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
    public async Task ShouldDelegateToBrokerWhenUnpack()
    {
        // Given
        DmsPath path = CreatePath(fullPath: "folder/archive");
        MemoryStream stream = new(buffer: [1, 2, 3]);

        dmsInstanceBrokerMock
            .Setup(expression: x => x.UnpackAsync(path: path, content: stream, ignoreArchiveRoot: true))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await dmsInstanceService.UnpackAsync(path: path, content: stream, ignoreArchiveRoot: true);

        // Then
        dmsInstanceBrokerMock.Verify(expression: x => x.UnpackAsync(path: path, content: stream, ignoreArchiveRoot: true), times: Times.Once);
        dmsInstanceBrokerMock.VerifyNoOtherCalls();
    }

}