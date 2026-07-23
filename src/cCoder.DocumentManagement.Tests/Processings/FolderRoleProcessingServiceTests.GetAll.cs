// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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

        folderRoleServiceMock.Setup(expression: x => x.GetAll())
            .Returns(value: queryableLinks);

        // When
        IQueryable<FolderRole> result = folderRoleProcessingService.GetAll();

        // Then
        result.Should()
            .BeSameAs(expected: queryableLinks);

        folderRoleServiceMock.Verify(expression: x => x.GetAll(), times: Times.Once);
        folderRoleServiceMock.VerifyNoOtherCalls();
        roleBrokerMock.VerifyNoOtherCalls();
        folderServiceMock.VerifyNoOtherCalls();
    }

}