// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using Moq;
using Xunit;
using DataFile = cCoder.Data.Models.DMS.File;
using FileEntity = cCoder.Data.Models.DMS.File;


namespace cCoder.Core.Services.Tests.DMS.Foundations;

public partial class FileServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGetAll()
    {
        // Given
        FileEntity file = CreateRandomFile();
        IQueryable<DataFile> files = new[] { ToExternalFile(file: file) }.AsQueryable();

        fileBrokerMock.Setup(expression: x => x.GetAllFiles(false)).Returns(value: files);

        // When
        IQueryable<FileEntity> result = fileService.GetAll();

        // Then
        result.Should().BeEquivalentTo(expectation: new[] { file }.AsQueryable());
        fileBrokerMock.Verify(expression: x => x.GetAllFiles(false), times: Times.Once);
        fileBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}