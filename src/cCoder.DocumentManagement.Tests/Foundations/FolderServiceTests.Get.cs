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
            .Setup(expression: x => x.SelectAllFolders(ignoreFilters: false))
            .Returns(value: new[] { ToExternalFolder(folder: folder) }.AsQueryable());

        // When
        Folder result = folderService.Get(id: folderId);

        // Then
        result.Should()
            .BeEquivalentTo(expectation: folder);

        folderBrokerMock.Verify(expression: x => x.SelectAllFolders(ignoreFilters: false), times: Times.Once);
        folderBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}