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
using DataFileContent = cCoder.Data.Models.DMS.FileContent;


namespace cCoder.Core.Services.Tests.DMS.Foundations;

public partial class FileContentServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGetAll()
    {
        // Given
        FileContent fileContent = CreateRandomFileContent();
        IQueryable<DataFileContent> fileContents = new[] { ToExternalFileContent(fileContent: fileContent) }.AsQueryable();

        fileContentBrokerMock.Setup(expression: x => x.GetAllFileContents(false)).Returns(value: fileContents);

        // When
        IQueryable<FileContent> result = fileContentService.GetAll();

        // Then
        result.Should().BeEquivalentTo(expectation: new[] { fileContent }.AsQueryable());
        fileContentBrokerMock.Verify(expression: x => x.GetAllFileContents(false), times: Times.Once);
        fileContentBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}