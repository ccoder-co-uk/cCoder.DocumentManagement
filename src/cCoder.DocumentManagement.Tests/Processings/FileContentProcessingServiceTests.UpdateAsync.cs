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
    public async Task ShouldDelegateToFoundationServiceWhenUpdateAsync()
    {
        // Given
        FileContent fileContent = CreateRandomFileContent();

        fileContentServiceMock
            .Setup(expression: x => x.UpdateFileContentAsync(fileContent: fileContent))
            .Returns(value: ValueTask.FromResult(result: fileContent));

        // When
        FileContent result = await fileContentProcessingService.UpdateFileContentAsync(entity: fileContent);

        // Then
        result.Should()
            .BeSameAs(expected: fileContent);

        fileContentServiceMock.Verify(expression: x => x.UpdateFileContentAsync(fileContent: fileContent), times: Times.Once);
        fileContentServiceMock.VerifyNoOtherCalls();
    }

}