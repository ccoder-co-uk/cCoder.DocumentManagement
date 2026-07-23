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
    public async Task ShouldDelegateToBrokerWhenSave()
    {
        // Given
        DmsPath path = CreatePath(fullPath: "folder/file.txt");
        MemoryStream stream = new(buffer: [4, 5, 6]);

        dmsInstanceBrokerMock.Setup(expression: x => x.SaveAsync(path: path, content: stream))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await dmsInstanceService.SaveAsync(path: path, content: stream);

        // Then
        dmsInstanceBrokerMock.Verify(expression: x => x.SaveAsync(path: path, content: stream), times: Times.Once);
        dmsInstanceBrokerMock.VerifyNoOtherCalls();
    }

}