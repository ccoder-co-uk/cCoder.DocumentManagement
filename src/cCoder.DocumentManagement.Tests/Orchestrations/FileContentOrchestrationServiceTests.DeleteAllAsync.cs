// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.DMS;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Orchestrations;

public partial class FileContentOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDelegateToProcessingServiceWhenDeleteAllAsync()
    {
        // Given
        FileContent[] entities = [CreateRandomFileContent()];

        fileContentProcessingServiceMock.Setup(expression: x => x.DeleteAllFileContentAsync(deletedFileContent: entities))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAllFileContentAsync(deletedFileContent: entities);

        // Then
        fileContentProcessingServiceMock.Verify(expression: x => x.DeleteAllFileContentAsync(deletedFileContent: entities), times: Times.Once);
        fileContentProcessingServiceMock.VerifyNoOtherCalls();
        fileContentEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}