// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
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
            .Setup(expression: x => x.RaiseFileContentAddEventAsync(entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseFileContentAddEventAsync(entity: entity);

        // Then
        fileContentEventServiceMock.Verify(expression: x => x.RaiseFileContentAddEventAsync(entity), times: Times.Once);
        fileContentEventServiceMock.VerifyNoOtherCalls();
    }

}