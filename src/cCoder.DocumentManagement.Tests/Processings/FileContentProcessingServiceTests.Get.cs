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


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FileContentProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGet()
    {
        // Given
        FileContent entity = CreateRandomFileContent();
        var id = entity.Id;

        fileContentServiceMock.Setup(expression: x => x.Get(fileContentId: id))
            .Returns(value: entity);

        // When
        FileContent result = fileContentProcessingService.Get(fileContentId: id);

        // Then
        result.Should()
            .BeSameAs(expected: entity);

        fileContentServiceMock.Verify(expression: x => x.Get(fileContentId: id), times: Times.Once);
        fileContentServiceMock.VerifyNoOtherCalls();
    }

}