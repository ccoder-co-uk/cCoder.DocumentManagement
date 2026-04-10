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
        DmsPath oldPath = CreatePath("folder/old.txt");
        DmsPath newPath = CreatePath("folder/new.txt");
        dmsInstanceBrokerMock.Setup(x => x.MoveAsync(oldPath, newPath)).Returns(ValueTask.CompletedTask);

        // When
        await dmsInstanceService.MoveAsync(oldPath, newPath);

        // Then
        dmsInstanceBrokerMock.Verify(x => x.MoveAsync(oldPath, newPath), Times.Once);
        dmsInstanceBrokerMock.VerifyNoOtherCalls();
    }

}




