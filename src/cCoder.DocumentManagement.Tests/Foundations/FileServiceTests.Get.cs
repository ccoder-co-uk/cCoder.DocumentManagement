using FluentAssertions;
using Moq;
using Xunit;
using FileEntity = cCoder.Data.Models.DMS.File;


namespace cCoder.Core.Services.Tests.DMS.Foundations;

public partial class FileServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGet()
    {
        // Given
        Guid fileId = Guid.NewGuid();
        FileEntity file = CreateRandomFile(id: fileId);

        fileBrokerMock
            .Setup(x => x.GetAllFiles(false))
            .Returns(new[] { ToExternalFile(file) }.AsQueryable());

        // When
        FileEntity result = fileService.Get(fileId);

        // Then
        result.Should().BeEquivalentTo(file);
        fileBrokerMock.Verify(x => x.GetAllFiles(false), Times.Once);
        fileBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}








