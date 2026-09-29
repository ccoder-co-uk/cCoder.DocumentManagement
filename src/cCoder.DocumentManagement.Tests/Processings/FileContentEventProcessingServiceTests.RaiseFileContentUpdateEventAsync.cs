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
    public async Task ShouldPassThroughCallWhenRaiseFileContentUpdateEventAsync()
    {
        // Given
        FileContent entity = CreateRandomFileContent();

        fileContentEventServiceMock
            .Setup(expression: x => x.RaiseFileContentUpdateEventAsync(fileContent: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseFileContentUpdateEventAsync(fileContent: entity);

        // Then
        fileContentEventServiceMock.Verify(expression: x => x.RaiseFileContentUpdateEventAsync(fileContent: entity), times: Times.Once);
        fileContentEventServiceMock.VerifyNoOtherCalls();
    }

}