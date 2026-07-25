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
using DataFolderRole = cCoder.Data.Models.Security.FolderRole;


namespace cCoder.Core.Services.Tests.DMS.Foundations;

public partial class FolderRoleServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGetAll()
    {
        // Given
        FolderRole folderRole = CreateRandomFolderRole();
        IQueryable<DataFolderRole> folderRoles = new[] { ToExternalFolderRole(folderRole: folderRole) }.AsQueryable();

        folderRoleBrokerMock.Setup(expression: x => x.SelectAllFolderRoles(ignoreFilters: false))
            .Returns(value: folderRoles);

        // When
        IQueryable<FolderRole> result = folderRoleService.GetAll();

        // Then
        result.Should()
            .BeEquivalentTo(expectation: new[] { folderRole }.AsQueryable());

        folderRoleBrokerMock.Verify(expression: x => x.SelectAllFolderRoles(ignoreFilters: false), times: Times.Once);
        folderRoleBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}