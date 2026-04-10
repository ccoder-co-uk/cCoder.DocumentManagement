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
        DmsPath path = CreatePath("folder/file.txt");
        dmsInstanceBrokerMock.Setup(x => x.DropAsync(path, 7)).Returns(ValueTask.CompletedTask);

        // When
        await dmsInstanceService.DropAsync(path, 7);

        // Then
        dmsInstanceBrokerMock.Verify(x => x.DropAsync(path, 7), Times.Once);
        dmsInstanceBrokerMock.VerifyNoOtherCalls();
    }

}




