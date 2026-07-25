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

public partial class FileContentProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGetAll()
    {
        // Given
        IQueryable<FileContent> entities = new[] { CreateRandomFileContent() }.AsQueryable();

        fileContentServiceMock.Setup(expression: x => x.GetAll())
            .Returns(value: entities);

        // When
        IQueryable<FileContent> result = fileContentProcessingService.GetAll();

        // Then
        result.Should()
            .BeSameAs(expected: entities);

        fileContentServiceMock.Verify(expression: x => x.GetAll(), times: Times.Once);
        fileContentServiceMock.VerifyNoOtherCalls();
    }

}