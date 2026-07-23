// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using Moq;
using Xunit;
using FileEntity = cCoder.Data.Models.DMS.File;


namespace cCoder.Core.Services.Tests.DMS.Foundations;

public partial class FileServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGet()
    {
        // Given
        Guid fileId = Guid.NewGuid();
        FileEntity file = CreateRandomFile(id: fileId);

        fileBrokerMock
            .Setup(expression: x => x.SelectAllFiles(ignoreFilters: false))
            .Returns(value: new[] { ToExternalFile(file: file) }.AsQueryable());

        // When
        FileEntity result = fileService.Get(id: fileId);

        // Then
        result.Should()
            .BeEquivalentTo(expectation: file);

        fileBrokerMock.Verify(expression: x => x.SelectAllFiles(ignoreFilters: false), times: Times.Once);
        fileBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}