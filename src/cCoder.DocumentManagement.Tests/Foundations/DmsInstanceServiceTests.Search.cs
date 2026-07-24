// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Foundations;

public partial class DmsInstanceServiceTests
{
    [Fact]
    public void ShouldReturnBrokerResultsWhenSearch()
    {
        // Given
        cCoder.Data.Models.DMS.File firstFile = CreateLocalFileAsync();
        cCoder.Data.Models.DMS.File secondFile = CreateLocalFileAsync();
        cCoder.Data.Models.DMS.File[] files = [firstFile, secondFile];

        dmsInstanceBrokerMock.Setup(expression: x => x.Search(needle: "term"))
            .Returns(value: files);

        // When
        IEnumerable<cCoder.Data.Models.DMS.File> returnedFiles =
            dmsInstanceService.Search(needle: "term");

        // Then
        returnedFiles.Should()
            .BeSameAs(expected: files);

        dmsInstanceBrokerMock.Verify(expression: x => x.Search(needle: "term"), times: Times.Once);
        dmsInstanceBrokerMock.VerifyNoOtherCalls();
    }

}