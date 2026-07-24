// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FileProcessingServiceTests
{
    [Fact]
    public async Task ShouldDeleteAllContentsWhenHandleFileDeleteEventAsync()
    {
        // Given
        cCoder.Data.Models.DMS.File file = CreateRandomFile();

        fileContentProcessingServiceMock
            .Setup(expression: x => x.DeleteAllForFileAsync(fileId: file.Id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await fileProcessingService.HandleFileDeleteEventAsync(file: file);

        // Then
        fileContentProcessingServiceMock.Verify(expression: x => x.DeleteAllForFileAsync(fileId: file.Id), times: Times.Once);
        fileContentProcessingServiceMock.VerifyNoOtherCalls();
        fileServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldDeleteEmptySetWhenFileHasNoContentsForHandleFileDeleteEventAsync()
    {
        // Given
        cCoder.Data.Models.DMS.File file = CreateRandomFile();

        fileContentProcessingServiceMock
            .Setup(expression: x => x.DeleteAllForFileAsync(fileId: file.Id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await fileProcessingService.HandleFileDeleteEventAsync(file: file);

        // Then
        fileContentProcessingServiceMock.Verify(expression: x => x.DeleteAllForFileAsync(fileId: file.Id), times: Times.Once);
        fileContentProcessingServiceMock.VerifyNoOtherCalls();
        fileServiceMock.VerifyNoOtherCalls();
    }

}