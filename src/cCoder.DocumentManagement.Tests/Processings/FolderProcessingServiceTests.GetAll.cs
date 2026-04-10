using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FolderProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGetAll()
    {
        // Given
        IQueryable<Folder> entities = new[] { CreateRandomFolder() }.AsQueryable();
        folderServiceMock.Setup(x => x.GetAll()).Returns(entities);

        // When
        IQueryable<Folder> result = folderProcessingService.GetAll();

        // Then
        result.Should().BeSameAs(entities);
        folderServiceMock.Verify(x => x.GetAll(), Times.Once);
        folderServiceMock.VerifyNoOtherCalls();
    }

}








