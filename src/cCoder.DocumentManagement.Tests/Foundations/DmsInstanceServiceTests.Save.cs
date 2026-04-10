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
        DmsPath path = CreatePath("folder/file.txt");
        MemoryStream stream = new([4, 5, 6]);
        dmsInstanceBrokerMock.Setup(x => x.SaveAsync(path, stream)).Returns(ValueTask.CompletedTask);

        // When
        await dmsInstanceService.SaveAsync(path, stream);

        // Then
        dmsInstanceBrokerMock.Verify(x => x.SaveAsync(path, stream), Times.Once);
        dmsInstanceBrokerMock.VerifyNoOtherCalls();
    }

}




