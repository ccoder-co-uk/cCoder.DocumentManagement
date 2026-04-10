using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Foundations;

public partial class FolderServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGet()
    {
        // Given
        Guid folderId = Guid.NewGuid();
        Folder folder = CreateRandomFolder(id: folderId);

        folderBrokerMock
            .Setup(x => x.GetAllFolders(false))
            .Returns(new[] { ToExternalFolder(folder) }.AsQueryable());

        // When
        Folder result = folderService.Get(folderId);

        // Then
        result.Should().BeEquivalentTo(folder);
        folderBrokerMock.Verify(x => x.GetAllFolders(false), Times.Once);
        folderBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}







