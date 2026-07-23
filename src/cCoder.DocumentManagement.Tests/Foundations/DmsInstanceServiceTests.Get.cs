// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using FluentAssertions;
using Moq;
using Xunit;
using DMSResult = cCoder.DocumentManagement.Models.DMSResult;
using DmsPath = cCoder.DocumentManagement.Models.Path;


namespace cCoder.Core.Services.Tests.DMS.Foundations;

public partial class DmsInstanceServiceTests
{
    [Fact]
    public void ShouldReturnBrokerResultWhenGet()
    {
        // Given
        DmsPath path = CreatePath(fullPath: "folder/file.txt");
        DMSResult result = CreateDmsResult(contentType: "text/plain");

        dmsInstanceBrokerMock.Setup(expression: x => x.Get(path: path, version: 3, search: "needle"))
            .Returns(value: result);

        // When
        DMSResult returnedResult = dmsInstanceService.Get(path: path, version: 3, search: "needle");

        // Then
        returnedResult.Should()
            .BeSameAs(expected: result);

        dmsInstanceBrokerMock.Verify(expression: x => x.Get(path: path, version: 3, search: "needle"), times: Times.Once);
        dmsInstanceBrokerMock.VerifyNoOtherCalls();
    }

}