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
        cCoder.Data.Models.DMS.File file = CreateRandomFile(id: id);

        fileServiceMock.Setup(expression: x => x.GetAll(ignoreFilters: true))
            .Returns(value: new[] { file }.AsQueryable());

        fileEventServiceMock
            .Setup(expression: x => x.RaiseFileDeleteEventAsync(entity: file))
            .Returns(value: ValueTask.CompletedTask);

        fileServiceMock.Setup(expression: x => x.DeleteAsync(fileId: id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await fileProcessingService.DeleteAsync(fileId: id);

        // Then
        fileServiceMock.Verify(expression: x => x.GetAll(ignoreFilters: true), times: Times.Once);
        fileServiceMock.Verify(expression: x => x.DeleteAsync(fileId: id), times: Times.Once);

        fileEventServiceMock.Verify(
            expression: x => x.RaiseFileDeleteEventAsync(entity: file),
            times: Times.Once);

        fileServiceMock.VerifyNoOtherCalls();
        fileEventServiceMock.VerifyNoOtherCalls();
    }

}