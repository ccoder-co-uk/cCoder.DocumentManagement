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
        DmsPath path = CreatePath("folder/archive");
        MemoryStream stream = new([1, 2, 3]);

        dmsInstanceBrokerMock
            .Setup(x => x.UnpackAsync(path, stream, true))
            .Returns(ValueTask.CompletedTask);

        // When
        await dmsInstanceService.UnpackAsync(path, stream, true);

        // Then
        dmsInstanceBrokerMock.Verify(x => x.UnpackAsync(path, stream, true), Times.Once);
        dmsInstanceBrokerMock.VerifyNoOtherCalls();
    }

}




