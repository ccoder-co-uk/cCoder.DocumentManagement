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
    public async Task ShouldDeleteEachItemWhenDeleteAllAsync()
    {
        // Given
        FileContent fileContent = CreateRandomFileContent();

        fileContentServiceMock
            .Setup(expression: x => x.DeleteAsync(id: fileContent.Id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await fileContentProcessingService.DeleteAllAsync(items: new[] { fileContent });

        // Then
        fileContentServiceMock.Verify(expression: x => x.DeleteAsync(id: fileContent.Id), times: Times.Once);
        fileContentServiceMock.VerifyNoOtherCalls();
    }

}