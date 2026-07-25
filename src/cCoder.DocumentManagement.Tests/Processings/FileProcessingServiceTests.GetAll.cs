// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FileProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGetAll()
    {
        // Given
        IQueryable<cCoder.Data.Models.DMS.File> files = new[]
        {
            CreateRandomFile(),
        }.AsQueryable();

        fileServiceMock.Setup(expression: x => x.GetAll())
            .Returns(value: files);

        // When
        IQueryable<cCoder.Data.Models.DMS.File> result = fileProcessingService.GetAll();

        // Then
        result.Should()
            .BeSameAs(expected: files);

        fileServiceMock.Verify(expression: x => x.GetAll(), times: Times.Once);
        fileServiceMock.VerifyNoOtherCalls();
    }

}