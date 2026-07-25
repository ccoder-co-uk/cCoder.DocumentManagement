// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using FluentAssertions;
using Moq;
using Xunit;
using DMSResult = cCoder.DocumentManagement.Dependencies.DMSResult;
using DmsPath = cCoder.DocumentManagement.Dependencies.Path;


namespace cCoder.Core.Services.Tests.DMS.Foundations;

public partial class DmsInstanceServiceTests
{
    [Fact]
    public void ShouldReturnBrokerResultWhenGetFilesZipped()
    {
        // Given
        DmsPath firstPath = CreatePath(fullPath: "folder/one.txt");
        DmsPath secondPath = CreatePath(fullPath: "folder/two.txt");
        DmsPath[] paths = [firstPath, secondPath];
        DMSResult result = CreateDmsResult(contentType: "application/zip");

        dmsInstanceBrokerMock.Setup(expression: x => x.GetFilesZipped(paths: paths))
            .Returns(value: result);

        // When
        DMSResult returnedResult = dmsInstanceService.GetFilesZipped(paths: paths);

        // Then
        returnedResult.Should()
            .BeSameAs(expected: result);

        dmsInstanceBrokerMock.Verify(expression: x => x.GetFilesZipped(paths: paths), times: Times.Once);
        dmsInstanceBrokerMock.VerifyNoOtherCalls();
    }

}