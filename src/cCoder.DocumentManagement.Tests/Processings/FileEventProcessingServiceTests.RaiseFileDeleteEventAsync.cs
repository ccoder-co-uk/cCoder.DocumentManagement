// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Moq;
using Xunit;
using FileEntity = cCoder.Data.Models.DMS.File;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FileEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseFileDeleteEventAsync()
    {
        // Given
        FileEntity entity = CreateRandomFileEntity();
        fileEventServiceMock
            .Setup(expression: x => x.RaiseFileDeleteEventAsync(entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseFileDeleteEventAsync(entity: entity);

        // Then
        fileEventServiceMock.Verify(expression: x => x.RaiseFileDeleteEventAsync(entity), times: Times.Once);
        fileEventServiceMock.VerifyNoOtherCalls();
    }

}