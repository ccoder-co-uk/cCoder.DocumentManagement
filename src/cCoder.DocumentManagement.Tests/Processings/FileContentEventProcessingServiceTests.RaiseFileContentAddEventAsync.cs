// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.DMS;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FileContentEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseFileContentAddEventAsync()
    {
        // Given
        FileContent entity = CreateRandomFileContent();

        fileContentEventServiceMock
            .Setup(expression: x => x.RaiseFileContentAddEventAsync(fileContent: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseFileContentAddEventAsync(fileContent: entity);

        // Then
        fileContentEventServiceMock.Verify(expression: x => x.RaiseFileContentAddEventAsync(fileContent: entity), times: Times.Once);
        fileContentEventServiceMock.VerifyNoOtherCalls();
    }

}