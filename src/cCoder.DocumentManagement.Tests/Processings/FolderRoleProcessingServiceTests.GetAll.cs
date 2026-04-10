using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FolderRoleProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGetAll()
    {
        // Given
        FolderRole[] links = [new() { FolderId = Guid.NewGuid(), RoleId = Guid.NewGuid() }];
        IQueryable<FolderRole> queryableLinks = links.AsQueryable();
        folderRoleServiceMock.Setup(x => x.GetAll()).Returns(queryableLinks);

        // When
        IQueryable<FolderRole> result = folderRoleProcessingService.GetAll();

        // Then
        result.Should().BeSameAs(queryableLinks);
        folderRoleServiceMock.Verify(x => x.GetAll(), Times.Once);
        folderRoleServiceMock.VerifyNoOtherCalls();
        roleBrokerMock.VerifyNoOtherCalls();
        folderServiceMock.VerifyNoOtherCalls();
    }

}








