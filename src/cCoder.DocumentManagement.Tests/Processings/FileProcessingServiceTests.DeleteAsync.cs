// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FileProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenDeleteAsync()
    {
        // Given
        Guid id = Guid.NewGuid();

        fileServiceMock.Setup(expression: x => x.DeleteAsync(fileId: id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await fileProcessingService.DeleteAsync(fileId: id);

        // Then
        fileServiceMock.Verify(expression: x => x.DeleteAsync(fileId: id), times: Times.Once);
        fileServiceMock.VerifyNoOtherCalls();
    }

}