using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FileContentProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenUpdateAsync()
    {
        // Given
        FileContent fileContent = CreateRandomFileContent();
        fileContentServiceMock
            .Setup(x => x.UpdateAsync(fileContent))
            .Returns(ValueTask.FromResult(fileContent));

        // When
        FileContent result = await fileContentProcessingService.UpdateAsync(fileContent);

        // Then
        result.Should().BeSameAs(fileContent);
        fileContentServiceMock.Verify(x => x.UpdateAsync(fileContent), Times.Once);
        fileContentServiceMock.VerifyNoOtherCalls();
    }

}








