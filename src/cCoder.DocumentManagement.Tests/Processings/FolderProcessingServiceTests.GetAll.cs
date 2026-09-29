// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.Data.Models.DMS;
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

        folderServiceMock.Setup(expression: x => x.GetAll())
            .Returns(value: entities);

        // When
        IQueryable<Folder> result = folderProcessingService.GetAll();

        // Then
        result.Should()
            .BeSameAs(expected: entities);

        folderServiceMock.Verify(expression: x => x.GetAll(), times: Times.Once);
        folderServiceMock.VerifyNoOtherCalls();
    }

}