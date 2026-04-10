using cCoder.DocumentManagement.Services.Processings;
using FluentAssertions;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class DmsProcessingServiceTests
{
    [Fact]
    public async Task ShouldReturnNoContentWhenMethodIsOptions()
    {
        // Given
        DmsProcessingRequest request = CreateRequest("OPTIONS", "/api/dms/folder");

        // When
        DmsProcessingResponse response = await dmsProcessingService.ProcessAsync(request);

        // Then
        response.StatusCode.Should().Be(204);
        response.ContentType.Should().Be("application/json");
        response.HasBody.Should().BeFalse();
        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }
}






