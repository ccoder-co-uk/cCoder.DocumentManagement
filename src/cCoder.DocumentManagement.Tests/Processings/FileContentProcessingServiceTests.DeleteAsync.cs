// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FileContentProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenDeleteAsync()
    {
        // Given
        Guid id = Guid.NewGuid();

        fileContentServiceMock.Setup(expression: x => x.DeleteAsync(id: id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await fileContentProcessingService.DeleteAsync(id: id);

        // Then
        fileContentServiceMock.Verify(expression: x => x.DeleteAsync(id: id), times: Times.Once);
        fileContentServiceMock.VerifyNoOtherCalls();
    }

}