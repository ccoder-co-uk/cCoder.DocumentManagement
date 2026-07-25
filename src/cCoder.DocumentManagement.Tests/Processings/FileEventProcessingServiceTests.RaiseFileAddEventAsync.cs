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
    public async Task ShouldPassThroughCallWhenRaiseFileAddEventAsync()
    {
        // Given
        FileEntity entity = CreateRandomFileEntity();

        fileEventServiceMock
            .Setup(expression: x => x.RaiseFileAddEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseFileAddEventAsync(entity: entity);

        // Then
        fileEventServiceMock.Verify(expression: x => x.RaiseFileAddEventAsync(entity: entity), times: Times.Once);
        fileEventServiceMock.VerifyNoOtherCalls();
    }

}