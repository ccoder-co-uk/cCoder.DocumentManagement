// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FileContentProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenAddAsync()
    {
        // Given
        FileContent fileContent = CreateRandomFileContent();
        fileContentServiceMock
            .Setup(expression: x => x.AddAsync(fileContent))
            .Returns(value: ValueTask.FromResult(fileContent));

        // When
        FileContent result = await fileContentProcessingService.AddAsync(entity: fileContent);

        // Then
        Assert.Same(expected: fileContent, actual: result);
        fileContentServiceMock.Verify(expression: x => x.AddAsync(fileContent), times: Times.Once);
        fileContentServiceMock.VerifyNoOtherCalls();
    }

}