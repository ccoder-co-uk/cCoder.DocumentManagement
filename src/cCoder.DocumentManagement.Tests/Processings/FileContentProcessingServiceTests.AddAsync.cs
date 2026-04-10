using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FileContentProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenAddAsync()
    {
        // Given
        FileContent fileContent = CreateRandomFileContent();
        fileContentServiceMock
            .Setup(x => x.AddAsync(fileContent))
            .Returns(ValueTask.FromResult(fileContent));

        // When
        FileContent result = await fileContentProcessingService.AddAsync(fileContent);

        // Then
        Assert.Same(fileContent, result);
        fileContentServiceMock.Verify(x => x.AddAsync(fileContent), Times.Once);
        fileContentServiceMock.VerifyNoOtherCalls();
    }

}








