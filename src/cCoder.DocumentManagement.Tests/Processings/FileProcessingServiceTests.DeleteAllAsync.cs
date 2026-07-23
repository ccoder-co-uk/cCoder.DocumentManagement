// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FileProcessingServiceTests
{
    [Fact]
    public async Task ShouldDeleteEachFileWhenDeleteAllAsync()
    {
        // Given
        cCoder.Data.Models.DMS.File file = CreateRandomFile();

        fileServiceMock.Setup(expression: x => x.DeleteAsync(id: file.Id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await fileProcessingService.DeleteAllAsync(items: new[] { file });

        // Then
        fileServiceMock.Verify(expression: x => x.DeleteAsync(id: file.Id), times: Times.Once);
        fileServiceMock.VerifyNoOtherCalls();
    }

}