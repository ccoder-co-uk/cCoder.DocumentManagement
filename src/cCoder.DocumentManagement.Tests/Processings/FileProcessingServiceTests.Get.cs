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
    public void ShouldDelegateToFoundationServiceWhenGet()
    {
        // Given
        cCoder.Data.Models.DMS.File file = CreateRandomFile();
        fileServiceMock.Setup(expression: x => x.Get(file.Id)).Returns(value: file);

        // When
        cCoder.Data.Models.DMS.File result = fileProcessingService.Get(id: file.Id);

        // Then
        result.Should().BeSameAs(expected: file);
        fileServiceMock.Verify(expression: x => x.Get(file.Id), times: Times.Once);
        fileServiceMock.VerifyNoOtherCalls();
    }

}