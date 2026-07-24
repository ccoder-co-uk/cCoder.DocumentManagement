// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
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

        fileContentProcessingServiceMock.Setup(expression: x => x.DeleteAllFileContentAsync(items: entities))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAllFileContentAsync(items: entities);

        // Then
        fileContentProcessingServiceMock.Verify(expression: x => x.DeleteAllFileContentAsync(items: entities), times: Times.Once);
        fileContentProcessingServiceMock.VerifyNoOtherCalls();
        fileContentEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}