using FluentAssertions;
using Moq;
using Xunit;
using DataFile = cCoder.Data.Models.DMS.File;
using FileEntity = cCoder.Data.Models.DMS.File;


namespace cCoder.Core.Services.Tests.DMS.Foundations;

public partial class FileServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGetAll()
    {
        // Given
        FileEntity file = CreateRandomFile();
        IQueryable<DataFile> files = new[] { ToExternalFile(file) }.AsQueryable();

        fileBrokerMock.Setup(x => x.GetAllFiles(false)).Returns(files);

        // When
        IQueryable<FileEntity> result = fileService.GetAll();

        // Then
        result.Should().BeEquivalentTo(new[] { file }.AsQueryable());
        fileBrokerMock.Verify(x => x.GetAllFiles(false), Times.Once);
        fileBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}









