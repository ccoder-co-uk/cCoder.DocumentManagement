// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.DMS;
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
            .Setup(expression: x => x.UpdateFileContentAsync(updatedFileContent: fileContent))
            .Returns(value: ValueTask.FromResult(result: fileContent));

        // When
        FileContent result = await fileContentProcessingService.UpdateFileContentAsync(updatedFileContent: fileContent);

        // Then
        result.Should()
            .BeSameAs(expected: fileContent);

        fileContentServiceMock.Verify(expression: x => x.UpdateFileContentAsync(updatedFileContent: fileContent), times: Times.Once);
        fileContentServiceMock.VerifyNoOtherCalls();
    }

}