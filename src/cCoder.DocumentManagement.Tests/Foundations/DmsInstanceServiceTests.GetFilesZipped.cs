// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using FluentAssertions;
using Moq;
using Xunit;
using DMSResult = cCoder.DocumentManagement.Models.DMSResult;


namespace cCoder.Core.Services.Tests.DMS.Foundations;

public partial class DmsInstanceServiceTests
{
    [Fact]
    public void ShouldReturnBrokerResultWhenGetFilesZipped()
    {
        // Given
        string[] paths = ["folder/one.txt", "folder/two.txt"];
        DMSResult result = CreateDmsResult(contentType: "application/zip");

        dmsInstanceBrokerMock.Setup(expression: x => x.GetFilesZipped(paths: paths))
            .Returns(value: result);

        // When
        DMSResult returnedResult = dmsInstanceService.GetFilesZipped(
            paths: paths);

        // Then
        returnedResult.Should()
            .BeSameAs(expected: result);

        dmsInstanceBrokerMock.Verify(expression: x => x.GetFilesZipped(paths: paths), times: Times.Once);
        dmsInstanceBrokerMock.VerifyNoOtherCalls();
    }

}