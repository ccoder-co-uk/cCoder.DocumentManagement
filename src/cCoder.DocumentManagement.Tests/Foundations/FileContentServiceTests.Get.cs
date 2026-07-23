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


namespace cCoder.Core.Services.Tests.DMS.Foundations;

public partial class FileContentServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGet()
    {
        // Given
        Guid fileContentId = Guid.NewGuid();
        FileContent fileContent = CreateRandomFileContent(id: fileContentId);

        fileContentBrokerMock
            .Setup(expression: x => x.SelectAllFileContents(ignoreFilters: false))
            .Returns(value: new[] { ToExternalFileContent(fileContent: fileContent) }.AsQueryable());

        // When
        FileContent result = fileContentService.Get(id: fileContentId);

        // Then
        result.Should()
            .BeEquivalentTo(expectation: fileContent);

        fileContentBrokerMock.Verify(expression: x => x.SelectAllFileContents(ignoreFilters: false), times: Times.Once);
        fileContentBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}