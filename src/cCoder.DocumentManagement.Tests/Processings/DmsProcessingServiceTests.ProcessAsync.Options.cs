// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        DmsProcessingRequest request = CreateRequest(method: "OPTIONS", requestPath: "/api/dms/folder");

        // When
        DmsProcessingResponse response = await dmsProcessingService.ProcessDmsProcessingRequestAsync(request: request);

        // Then
        response.StatusCode.Should()
            .Be(expected: 204);

        response.ContentType.Should()
            .Be(expected: "application/json");

        response.HasBody.Should()
            .BeFalse();

        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }
}